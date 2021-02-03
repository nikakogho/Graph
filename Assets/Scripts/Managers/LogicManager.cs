using UnityEngine;
using System.Collections.Generic;

public class LogicManager : MonoBehaviour
{
    public delegate double OrderFunction(double a, double b);
    public delegate double Function(double x);
    public static Function function;

    static Dictionary<string, Function> methods = new Dictionary<string, Function>()
    {
        {
            "ABS",
            (x) => { return System.Math.Abs(x); }
        },

        {
            "SQRT",
            (x) => { return System.Math.Sqrt(x); }
        }
    };

    static Dictionary<char, OrderFunction> orders = new Dictionary<char, OrderFunction>()
    {
        {
            '+',
            (a, b) => { return a + b; }
        },

        {
            '-',
            (a, b) => { return a - b; }
        },

        {
            '*',
            (a, b) => { return a * b; }
        },

        {
            '/',
            (a, b) => { return a / b; }
        },

        {
            '^',
            (a, b) => { return System.Math.Pow(a, b); }
        }
    };
    
    enum CurrentlyTaking { Number, Order, Method, PostOrder, BracketFunction }

    void Awake()
    {
        function = (x) => { return x; };
    }
    
    static void FinishUp(ref Function oldFunc, OrderFunction currentOrder, string currentMethod, Function newFunc, bool negativeNumber)
    {
        var realOld = new Function(oldFunc);
        var actualMethod = methods.GetFunction(currentMethod);
        double multiplier = negativeNumber ? -1 : 1;

        oldFunc = (x) => { return currentOrder.Invoke(realOld(x), actualMethod(newFunc(x * multiplier))); };
    }

    public static bool UpdateFunction(string methodString, ref Function function)
    {
        function = (x) => { return 0; };

        try
        {
            #region Method From String

            #region Stored Data

            bool dotted = false;

            Function numberFunction = null;

            CurrentlyTaking currentlyTaking = CurrentlyTaking.PostOrder;
            OrderFunction currentOrder = orders['+'];
            string currentMethod = string.Empty;

            bool negativeNumber = false;

            string currentNumber = string.Empty;

            #endregion

            #region For Loop

            for (int i = 0; i < methodString.Length; i++)
            {
                char c = methodString[i];

                switch (c)
                {
                    #region Number Cases

                    case '0':
                    case '1':
                    case '2':
                    case '3':
                    case '4':
                    case '5':
                    case '6':
                    case '7':
                    case '8':
                    case '9':

                        if (currentlyTaking == CurrentlyTaking.PostOrder)
                        {
                            currentlyTaking = CurrentlyTaking.Number;
                        }

                        if (currentlyTaking != CurrentlyTaking.Number || currentNumber == "x") return false;

                        currentNumber += c;

                        break;

                    #endregion

                    case '.':
                        if (dotted) return false;
                        if (currentlyTaking != CurrentlyTaking.Number) return false;

                        dotted = true;

                        break;

                    #region Letter Cases

                    case 'A':
                    case 'B':
                    case 'C':
                    case 'D':
                    case 'E':
                    case 'F':
                    case 'G':
                    case 'H':
                    case 'I':
                    case 'J':
                    case 'K':
                    case 'L':
                    case 'M':
                    case 'N':
                    case 'O':
                    case 'P':
                    case 'Q':
                    case 'R':
                    case 'S':
                    case 'T':
                    case 'U':
                    case 'V':
                    case 'W':
                    case 'X':
                    case 'Y':
                    case 'Z':

                        if (currentlyTaking == CurrentlyTaking.PostOrder)
                        {
                            currentlyTaking = CurrentlyTaking.Method;
                        }
                        else if (currentlyTaking != CurrentlyTaking.Method) return false;

                        currentMethod += c;

                        break;

                    #endregion

                    case 'x':
                        if (currentlyTaking == CurrentlyTaking.PostOrder) currentlyTaking = CurrentlyTaking.Number;
                        if (currentlyTaking != CurrentlyTaking.Number || currentNumber != string.Empty) return false;

                        currentNumber = "x";

                        break;

                    #region Action Cases

                    case '+':
                        goto case '^';

                    case '-':
                        if (currentlyTaking == CurrentlyTaking.Order) goto case '^';

                        switch (currentlyTaking)
                        {
                            case CurrentlyTaking.PostOrder:
                                currentlyTaking = CurrentlyTaking.Number;
                                goto case CurrentlyTaking.Number;

                            case CurrentlyTaking.Number:
                                if (currentNumber != string.Empty) return false;

                                negativeNumber = !negativeNumber;
                                break;

                            default:
                                return false;
                        }

                        break;

                    case '*':
                        goto case '^';

                    case '/':
                        goto case '^';

                    case '^':
                        if (currentOrder == null)
                            currentOrder = orders[c];
                        else return false;

                        break;

                    #endregion

                    case '(':
                        if (currentlyTaking == CurrentlyTaking.PostOrder) currentlyTaking = CurrentlyTaking.Number;

                        if (currentlyTaking != CurrentlyTaking.Number) return false;
                        if (currentNumber != string.Empty) return false;

                        currentlyTaking = CurrentlyTaking.BracketFunction;

                        string insideBrackets = string.Empty;

                        int mustFindCloses = 1;

                        int j;

                        for (j = i + 1; j < methodString.Length; j++)
                        {
                            switch (methodString[j])
                            {
                                case '(':
                                    mustFindCloses++;
                                    break;

                                case ')':
                                    mustFindCloses--;
                                    break;
                            }

                            if (mustFindCloses == 0) break;

                            insideBrackets += methodString[j];
                        }
                        
                        methodString = methodString.Remove(i + 1, j - i - 1);

                        if (UpdateFunction(insideBrackets, ref numberFunction))
                        {

                        }
                        else return false;

                        break;

                    case ')':
                        if (i < 1 || methodString[i - 1] != '(') return false;
                        break;

                    case ' ':
                        switch (currentlyTaking)
                        {
                            case CurrentlyTaking.Order:
                                currentlyTaking = CurrentlyTaking.PostOrder;
                                break;

                            case CurrentlyTaking.PostOrder:
                                break;

                            case CurrentlyTaking.Method:
                                currentlyTaking = CurrentlyTaking.Number;
                                break;

                            case CurrentlyTaking.Number:
                                if (currentNumber == "x")
                                {
                                    numberFunction = (x) => { return x; };
                                }
                                else
                                {
                                    numberFunction = (x) => { return double.Parse(currentNumber); };
                                }

                                goto case CurrentlyTaking.BracketFunction;

                            case CurrentlyTaking.BracketFunction:
                                FinishUp(ref function, currentOrder, currentMethod, numberFunction, negativeNumber);

                                currentlyTaking = CurrentlyTaking.Order;

                                negativeNumber = false;
                                dotted = false;
                                currentNumber = string.Empty;
                                currentMethod = string.Empty;
                                numberFunction = null;
                                currentOrder = null;

                                break;
                        }

                        break;

                    default: return false;
                }
            }

            #endregion

            #region Left Over

            switch (currentlyTaking)
            {
                case CurrentlyTaking.Number:
                    if (currentNumber == "x")
                    {
                        numberFunction = (x) => { return x; };
                    }
                    else
                    {
                        numberFunction = (x) => { return double.Parse(currentNumber); };
                    }

                    goto case CurrentlyTaking.BracketFunction;

                case CurrentlyTaking.BracketFunction:
                    FinishUp(ref function, currentOrder, currentMethod, numberFunction, negativeNumber);
                    break;

                default:
                    return false;
            }

            #endregion

            #endregion
        }
        catch(System.Exception e)
        {
            Debug.LogWarning(e.Message);
            return false;
        }

        return true;
    }
}
