using Bank.Application.Features.Accounts.Dtos;
using Bank.Application.Interfaces;
using Bank.Domain.Constants;
using Bank.Domain.Entities;
using Bank.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Infrastructure.Services
{
	public class AuthService : IAuthService
	{
		private readonly UserManager<Account> _userManager;
		private readonly IConfiguration _configuration;
		private readonly IUserRepository _userRepository;
		private readonly IRefreshTokenRepository _refreshTokenRepository;

		public AuthService(
			UserManager<Account> userManager,
			IConfiguration configuration,
			IUserRepository userRepository,
			IRefreshTokenRepository refreshTokenRepository)
		{
			_userManager = userManager;
			_configuration = configuration;
			_userRepository = userRepository;
			_refreshTokenRepository = refreshTokenRepository;
		}

		public async Task<AuthResponseDto> RegisterAsync(string username, string email, string password, string role = "User")
		{
			if (await _userRepository.ExistsByEmailAsync(email, default))
				throw new Exception("Email already in use.");
			var user = new Account
			{
				UserName = username,
				Email = email
			};
			var result = await _userManager.CreateAsync(user, password);

			if (!result.Succeeded)
				throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

			var assignedRole = role == Roles.Admin ? Roles.Admin : Roles.User;
			await _userManager.AddToRoleAsync(user, assignedRole);

			return await GenerateJwtAndRefreshToken(user);
		}

		public async Task<AuthResponseDto> LoginAsync(string username, string password)
		{
			var user = await _userRepository.GetByUsernameAsync(username, default);
			if (user == null || !await _userManager.CheckPasswordAsync(user, password))
				throw new UnauthorizedAccessException("Invalid credentials.");

			return await GenerateJwtAndRefreshToken(user);
		}

		public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
		{
			var tokenEntity = await _refreshTokenRepository.GetByTokenAsync(refreshToken, default);

			if (tokenEntity == null)
				throw new SecurityTokenException("Refresh token not found.");

			if (tokenEntity.Revoked)
				throw new SecurityTokenException("Refresh token has been revoked.");

			if (tokenEntity.ExpiresAt <= DateTime.UtcNow)
				throw new SecurityTokenException("Refresh token has expired.");

			// Step 2: Get the user
			var user = await _userManager.FindByIdAsync(tokenEntity.AccountId);
			if (user == null)
				throw new SecurityTokenException("User not found.");

			var newAccessToken = await GenerateAccessToken(user);

			return new AuthResponseDto(
				Id: user.Id,
				AccessToken: newAccessToken,
				RefreshToken: refreshToken,
				ExpiresAt: DateTime.UtcNow.AddMinutes(5)
			);
		}

		private async Task<string> GenerateAccessToken(Account user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

			var claims = new List<Claim>
	{
		new Claim(ClaimTypes.Name, user.UserName),
		new Claim(ClaimTypes.NameIdentifier, user.Id)
	};

			var roles = await _userManager.GetRolesAsync(user);
			claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(5),
				Issuer = _configuration["Jwt:Issuer"],
				Audience = _configuration["Jwt:Audience"],
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(token);
		}

		private async Task<AuthResponseDto> GenerateJwtAndRefreshToken(Account user)
		{
			var tokenHandler = new JwtSecurityTokenHandler();
			var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

			var claims = new List<Claim>
		{
			new Claim(ClaimTypes.Name, user.UserName),
			new Claim(ClaimTypes.NameIdentifier, user.Id)

		};

			var roles = await _userManager.GetRolesAsync(user);
			claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(claims),
				Expires = DateTime.UtcNow.AddMinutes(5),
				Issuer = _configuration["Jwt:Issuer"],
				Audience = _configuration["Jwt:Audience"],
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
			};

			var token = tokenHandler.CreateToken(tokenDescriptor);
			var jwtToken = tokenHandler.WriteToken(token);

			var refreshToken = new RefreshToken
			{
				Token = Guid.NewGuid().ToString(),
				AccountId = user.Id,
				ExpiresAt = DateTime.UtcNow.AddDays(7),
				CreatedAt = DateTime.UtcNow,
				Revoked = false
			};

			await _refreshTokenRepository.AddAsync(refreshToken, default);

			return new AuthResponseDto(
				Id: user.Id,
				AccessToken: jwtToken,
				RefreshToken: refreshToken.Token,
				ExpiresAt: token.ValidTo
			);
		}
	}
}
