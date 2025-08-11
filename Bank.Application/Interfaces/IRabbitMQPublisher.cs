using Bank.Shared.Events;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank.Application.Interfaces
{
	public interface IRabbitMQPublisher
	{
		void Publish(TransactionCreatedEvent @event);
	}
}
