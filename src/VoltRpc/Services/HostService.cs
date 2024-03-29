﻿namespace VoltRpc.Services;

internal struct HostService
{
    public string InterfaceName { get; set; }

    public object InterfaceObject { get; set; }

    public ServiceMethod[] ServiceMethods { get; set; }
}