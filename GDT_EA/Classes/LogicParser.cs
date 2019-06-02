using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GDT_EA.Classes
{
    class LogicParser
    {
        private const char groupStart = '(';
        private const char groupEnd = ')';

        private const char logicalAnd = '&';
        private const char logicalOr = '|';


        private const string operatorEqual = "=";
        private const string operatorGreate = ">";
        private const string operatorLess = "<";
        private const string operatorNotEqual = "<>";


        internal bool Check(string content/*, Dictionary<string, string> variables*/)
        {
            var operations = new[] { operatorEqual, operatorNotEqual, operatorGreate, operatorLess };
            //var dic = new List<CalculatedCondition>();

            content = groupStart + content + groupEnd;

            foreach (var operation in operations)
            {
                var index = 0;
                do
                {
                    index = content.IndexOf(operation, index);
                    if (index == -1)
                    {
                        break;
                    }

                   // if (!dic.Any(c => index > c.BeginIndex && index < c.EndIndex))
                   // {
                        int beginIndex = GetExpressionBeginBoundary(content, index, operation.Length);
                        int endIndex = GetExpressionEndBoundary(content, index, operation.Length);

                        var expression = content.Substring(beginIndex, endIndex - beginIndex);
                       // var result = this.CheckCondition(expression, variables);

                       // dic.Add(new CalculatedCondition { BeginIndex = beginIndex, EndIndex = endIndex, Expression = expression, Result = result });
                   // }

                    index += operation.Length;

                } while (true);
            }

            //content = dic.Aggregate(content, (current, calculatedCondition) => current.Replace(calculatedCondition.Expression, calculatedCondition.Result ? "1" : "0"));
           // content = content.Replace(" ", string.Empty);

            return ComposeResult(content);
        }

        internal bool CheckCondition(string condition, Dictionary<string, string> variables)
        {
            var result = false;

            if (condition.IndexOfAny(new[] { logicalAnd, logicalOr }) != -1)
            {
                // only expressions like "varname > value" are supported
                throw new NotSupportedException();
            }

            var operations = new[] { operatorEqual, operatorNotEqual, operatorGreate, operatorLess };

            foreach (var operation in operations)
            {
                if (condition.Contains(operation))
                {
                    int index = condition.IndexOf(operation);
                    var verb = condition.Substring(0, index).Trim();

                    var arg = condition.Substring(index + operation.Length).Trim(new[] { ' ', '\'', '\"' });

                    string val = null;
                    if (variables.ContainsKey(verb))
                    {
                        val = variables[verb];
                    }

                    if (val != null)
                    {
                        decimal num1 = 0, num2 = 0;
                        bool isNum1 = decimal.TryParse(val, out num1);
                        bool isNum2 = decimal.TryParse(arg, out num2);

                        switch (operation)
                        {
                            case operatorEqual:
                                result = isNum1 && isNum2 ? num1 == num2 : val == arg;
                                break;
                            case operatorNotEqual:
                                result = isNum1 && isNum2 ? num1 != num2 : val != arg;
                                break;
                            case operatorGreate:
                                result = !isNum1 || !isNum2 || num1 > num2;
                                break;
                            case operatorLess:
                                result = !isNum1 || !isNum2 || num1 < num2;
                                break;
                        }
                        break;
                    }
                    else
                    {
                        throw new NotSupportedException(
string.Format("Variable {0} doesn't exists in the current context", verb));
                    }
                }
            }

            return result;
        }

        private bool ComposeResult(string content)
        {
            // (0|(1&1))&1)

            var start = 0;
            var resultChanged = false;

            do
            {
                start = 0;
                resultChanged = false;

                while (true)
                {
                    start = content.IndexOf(groupStart, start);

                    var nextStart = content.IndexOf(groupStart, start + 1);
                    var end = content.IndexOf(groupEnd, start + 1);

                    if (start == -1 && end == -1)
                    {
                        break;
                    }
                    else
                    {
                        if (end < nextStart || nextStart == -1)
                        {
                            var str = content.Substring(start, (end + 1) - start);
                            bool result = ToBool(str);
                            content = content.Replace(str, result ? "1" : "0");
                            resultChanged = true;
                            break;
                        }

                    }

                    start = nextStart;
                }

            } while (resultChanged && start != -1);

            return content == "1";
        }


        internal int GetExpressionBeginBoundary(string content, int operationIndex, int operationLength)
        {
            int resultIndex = -1;
            var index = operationIndex - 1;
            while (content.ElementAt(index) == ' ')
            {
                --index;
            }

            for (; index >= 0; --index)
            {
                var separatorsForNotStrings = new[] { ' ', groupStart, logicalAnd, logicalOr };
                var currentElement = content.ElementAt(index);
                if (groupEnd == currentElement)
                {
                    throw new NotSupportedException(string.Format("Illegal endGroup symbol found at {0}", index));
                }

                if (separatorsForNotStrings.Any(c => c == currentElement))
                {
                    resultIndex = index + 1;
                    break;
                }
            }

            // set begin on expression
            if (resultIndex == -1)
            {
                resultIndex = 0;
            }

            return resultIndex;
        }


        internal int GetExpressionEndBoundary(string content, int operationIndex, int operationLength)
        {
            int resultIndex = -1;
            // skip operation text
            var index = operationIndex + operationLength;
            // skip spaces if present
            while (content.ElementAt(index) == ' ')
            {
                ++index;
            }

            // check if operand is string
            bool isString = false;
            var argumentStartsWith = content.ElementAt(index);
            if (argumentStartsWith == '\'' || argumentStartsWith == '\"')
            {
                isString = true;
                ++index;
            }

            for (; index < content.Length; index++)
            {
                var separatorsForNotStrings = new[] { ' ', groupEnd, logicalAnd, logicalOr };
                var separatorsForStrings = new[] { '\'', '\"' };

                var currentElement = content.ElementAt(index);

                if (groupStart == currentElement)
                {
                    throw new NotSupportedException(string.Format("Illegal startGroup symbol found at {0}", index));
                }

                if (isString)
                {
                    if (separatorsForStrings.Any(c => c == currentElement))
                    {
                        ++index;
                        resultIndex = index;
                        break;
                    }
                }
                else
                {
                    if (separatorsForNotStrings.Any(c => c == currentElement))
                    {
                        resultIndex = index;
                        break;
                    }
                }
            }

            // set end on expression
            if (resultIndex == -1)
            {
                resultIndex = content.Length;
            }

            return resultIndex;

        }

        private bool ToBool(string str)
        {
            str = str.Trim(new[] { groupStart, groupEnd });

            var operations = new[] { logicalAnd, logicalOr };

            if (!operations.Any(o => str.Contains(o)))
            {
                return str == "1";
            }

            var arg1 = str.ElementAt(0) == '1';
            int iLength = str.Length;
            for (int i = 1; i<iLength; i += 2)
            {
                var operation = str.ElementAt(i);
                var arg2 = str.ElementAt(i + 1) == '1';

                switch (operation)
                {
                    case logicalAnd:
                        arg1 = arg1 & arg2;
                        break;
                    case logicalOr:
                        arg1 = arg1 | arg2;
                        break;
                }
            }

            return arg1;
        }

    }
}
