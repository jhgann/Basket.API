﻿using RabbitMQ.Client;
using System;

namespace RabbitMQEventBus
{
    public interface IRabbitMQPersistentConnection : IDisposable
    {
        bool IsConnected { get; }

        bool TryConnect();

        IModel CreateModel();
    }
}
