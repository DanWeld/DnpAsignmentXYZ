using System;

namespace EfcRepositories;

public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

