﻿namespace HobbyService.EventProcessing;

public interface IEventProcessor
{
    void ProcessEvent(string message);
}