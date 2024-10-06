﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public interface IRotable
    {
        int GetDirection();
        int GetAngularVelocity();
        void SetDirection(int newV);
        int GetDirectionsNumber();
    }
}