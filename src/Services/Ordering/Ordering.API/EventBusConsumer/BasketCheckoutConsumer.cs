using AutoMapper;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.Commands.CheckoutOrder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ordering.API.EventBusConsumer
{
    public class BasketCheckoutConsumer : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMapper _mapper;
        private readonly IMediator mediator;
        private readonly ILogger<BasketCheckoutConsumer> _logger;

        public BasketCheckoutConsumer(IMapper mapper, IMediator mediator, ILogger<BasketCheckoutConsumer> logger)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var command = _mapper.Map<CheckoutOrderCommand>(context.Message);
            var result = await mediator.Send(command);

            _logger.LogInformation("BaskerCheckoutCommand consumer successfully. Order Created with ID : {newOrderId}", result);
        }
    }
}
