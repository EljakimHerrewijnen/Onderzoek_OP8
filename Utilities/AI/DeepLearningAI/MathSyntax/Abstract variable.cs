﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MathSyntax
{
    abstract class Abstract_variable : SyntaxBlock
    {
        protected ArgumentValue Argument;
        bool isAlwaysConstant;

    
        public abstract SyntaxBlock Derivative(ArgumentValue ArgumentToDerive);

        protected Abstract_variable(ArgumentValue Argument, bool alwaysConstant)
        {
            this.isAlwaysConstant = alwaysConstant;
            this.Argument = Argument;
        }
        public abstract List<ArgumentValue> GetAllVariables(bool OnlyNonConstants);

        public abstract bool IsConstant(ArgumentValue Non_Constant);

        public SyntaxBlock Simplify()
        {
            return this;
        }

        public string print()
        {
            return Argument.Name;
        }

        public double Calculate()
        {
            return Argument.Value;
        }

        public abstract XElement Serialize();
    }
}
