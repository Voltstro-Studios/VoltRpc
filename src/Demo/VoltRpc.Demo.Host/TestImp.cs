﻿using System;
using VoltRpc.Demo.Shared;

namespace VoltRpc.Demo.Host
{
    public class TestImp : ITest
    {
        public void BasicTest()
        {
            Console.WriteLine("Basic Test!");
        }

        public void ParmTest(string message)
        {
            if (message == null)
            {
                Console.WriteLine("The message was null");
                return;
            }
            
            Console.WriteLine(message);
        }

        public string ReturnTest()
        {
            return "Hello Back!";
        }
    }
}