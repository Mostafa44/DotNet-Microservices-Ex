using System.Text;
using CommandsService.EventProcessing;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices
{
    public class MessageBusSubscriber : BackgroundService
    {
        private readonly IOptions<RabbitMQOptions> _rabbitMQOptions;
        private readonly IEventProcessor _eventProcessor;
        private IConnection _connection;
        private IModel _channel;
        private string _queueName;

        public MessageBusSubscriber(IOptions<RabbitMQOptions> rabbitMQOptions,
                                    IEventProcessor eventProcessor)
        {
            _rabbitMQOptions= rabbitMQOptions;
            _eventProcessor= eventProcessor;
            InitializeRabbitMQ();
        }

        private void InitializeRabbitMQ()
        {
            var factory= new ConnectionFactory()
            {
                HostName= _rabbitMQOptions.Value.Host,
                Port= int.Parse( _rabbitMQOptions.Value.Port)
            };
            _connection= factory.CreateConnection();
            _channel= _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _queueName= _channel.QueueDeclare().QueueName;
            _channel.QueueBind(queue : _queueName, exchange: "trigger", routingKey:"");
            Console.WriteLine("---> Listening on the Message Bus....");
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

        }

        private void RabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
        {
            Console.WriteLine("---> Connection Shutdown");
        }

        public override void Dispose()
        {
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
            base.Dispose();
        }


        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            stoppingToken.ThrowIfCancellationRequested();
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (ModuleHandle, ea)=>
            {
                Console.WriteLine(" ---> Event Received");
                var body = ea.Body;
                var notificationMessage= Encoding.UTF8.GetString(body.ToArray());
                _eventProcessor.ProcessEvent(notificationMessage);
            };

            _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);
            return Task.CompletedTask;

        }
    }
}