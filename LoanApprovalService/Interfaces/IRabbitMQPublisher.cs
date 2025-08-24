namespace LoanApprovalService.Interfaces
{
	public interface IRabbitMQPublisher
	{
		void Publish<T>(T @event) where T : class;
	}
}
