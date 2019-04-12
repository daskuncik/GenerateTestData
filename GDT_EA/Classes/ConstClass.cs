using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT_EA.Classes
{
    static class ConstClass
    {
        public enum QuaTypes
        {
            Extern = 1,
            Const,
            Static,
            Define,
            Struct,
            TypeDef
        }
       
        public static  Dictionary<int, string> Qualifiers = new  Dictionary<int, string>()
        {
            {(int)QuaTypes.Extern,"extern" },
            {(int)QuaTypes.Const, "const" },
            {(int)QuaTypes.Static, "static"},
            {(int)QuaTypes.Define, "#define" },
            {(int)QuaTypes.Struct, "struct" },
            {(int)QuaTypes.TypeDef, "typedef"}
        };

        public enum TokensEnum
        {
            InputPar,
            Declaration,
            Init, //Assigment
            InitDefault,
            FunctionArgList,
            VarName,
            VarType,
            Condition,
            ConditionBody,
            Else,
            Return,
            Increment
        }
        public const string semicolon = ";";
        public const string _if = "if";
        public const string eq = "=";
        public const string ret = "return";
        public const string inc = "++";
    }
}
