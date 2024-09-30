using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Group7_Assignment1_Calculator
{
    // Supporting classes //

    // Token class to represent numbers, operators, and parentheses in the expression
    public class Token
    {
        public enum TokenType { Number, Operator, LeftBracket, RightBracket }
        public TokenType Type { get; }
        public string Value { get; }

        public Token(TokenType type, string value)
        {
            Type = type;
            Value = value;
        }
    }

    // Operator class to define operations and their behavior
    public class Operator
    {
        public int Precedence { get; }
        public bool IsLeftAssociative { get; }

        public Operator(int precedence, bool isLeftAssociative)
        {
            Precedence = precedence;
            IsLeftAssociative = isLeftAssociative;
        }

        public double Apply(double left, double right, char operation)
        {
            switch (operation)
            {
                case '+': return left + right;
                case '-': return left - right;
                case '*': return left * right;
                case '/': return left / right;
                default: throw new Exception("Invalid operator");
            }
        }
    }

    // ExpressionEvaluator class to parse and evaluate mathematical expressions
    public class ExpressionEvaluator
    {
        private Dictionary<char, Operator> operators = new Dictionary<char, Operator>
        {
            { '+', new Operator(1, true) },
            { '-', new Operator(1, true) },
            { '*', new Operator(2, true) },
            { '/', new Operator(2, true) }
        };

        public double Evaluate(string expression)
        {
            var tokens = Parse(expression); // Step 1: Convert expression to tokens
            var postfixTokens = ToPostfix(tokens); // Step 2: Convert infix to postfix
            return EvaluatePostfix(postfixTokens); // Step 3: Evaluate the postfix expression
        }

        // Step 1: Parse the expression into tokens
        private List<Token> Parse(string expression)
        {
            List<Token> tokens = new List<Token>();
            int i = 0;

            while (i < expression.Length)
            {
                char current = expression[i];

                if (char.IsWhiteSpace(current))
                {
                    i++;
                    continue;
                }

                if (char.IsDigit(current) || current == '.')
                {
                    string number = "";
                    while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                    {
                        number += expression[i];
                        i++;
                    }
                    tokens.Add(new Token(Token.TokenType.Number, number));
                }
                else if ("+-*/".Contains(current))
                {
                    // Check for negative numbers
                    if (current == '-' && (tokens.Count == 0 || tokens[tokens.Count - 1].Type == Token.TokenType.Operator || tokens[tokens.Count - 1].Type == Token.TokenType.LeftBracket))
                    {
                        // Negative number
                        string number = "-";
                        i++;
                        while (i < expression.Length && (char.IsDigit(expression[i]) || expression[i] == '.'))
                        {
                            number += expression[i];
                            i++;
                        }
                        tokens.Add(new Token(Token.TokenType.Number, number));
                    }
                    else
                    {
                        tokens.Add(new Token(Token.TokenType.Operator, current.ToString()));
                        i++;
                    }
                }
                else if (current == '(')
                {
                    tokens.Add(new Token(Token.TokenType.LeftBracket, "("));
                    i++;
                }
                else if (current == ')')
                {
                    tokens.Add(new Token(Token.TokenType.RightBracket, ")"));
                    i++;
                }
                else
                {
                    throw new Exception("Invalid character in expression");
                }
            }

            return tokens;
        }

        // Step 2: Convert infix expression to postfix using Shunting-yard algorithm
        private List<Token> ToPostfix(List<Token> tokens)
        {
            Stack<Token> operatorStack = new Stack<Token>();
            List<Token> outputQueue = new List<Token>();

            foreach (var token in tokens)
            {
                if (token.Type == Token.TokenType.Number)
                {
                    outputQueue.Add(token);
                }
                else if (token.Type == Token.TokenType.Operator)
                {
                    while (operatorStack.Count > 0 && operatorStack.Peek().Type == Token.TokenType.Operator
                        && operators[operatorStack.Peek().Value[0]].Precedence >= operators[token.Value[0]].Precedence)
                    {
                        outputQueue.Add(operatorStack.Pop());
                    }
                    operatorStack.Push(token);
                }
                else if (token.Type == Token.TokenType.LeftBracket)
                {
                    operatorStack.Push(token);
                }
                else if (token.Type == Token.TokenType.RightBracket)
                {
                    while (operatorStack.Peek().Type != Token.TokenType.LeftBracket)
                    {
                        outputQueue.Add(operatorStack.Pop());
                    }
                    operatorStack.Pop(); // Remove the left bracket
                }
            }

            while (operatorStack.Count > 0)
            {
                outputQueue.Add(operatorStack.Pop());
            }

            return outputQueue;
        }

        // Step 3: Evaluate the postfix expression
        private double EvaluatePostfix(List<Token> tokens)
        {
            Stack<double> values = new Stack<double>();

            foreach (var token in tokens)
            {
                if (token.Type == Token.TokenType.Number)
                {
                    values.Push(double.Parse(token.Value));
                }
                else if (token.Type == Token.TokenType.Operator)
                {
                    double right = values.Pop();
                    double left = values.Pop();
                    char op = token.Value[0];
                    values.Push(operators[op].Apply(left, right, op));
                }
            }

            return values.Pop();
        }
    }

    public class Program
    {

        public static double ans = 0;
        // ProcessCommand method evaluates user input expression
        public static string ProcessCommand(string input)
        {
            try
            {
                input = input.Replace("ans", ans.ToString()); // replace previous result to 'ans'

                ExpressionEvaluator evaluator = new ExpressionEvaluator();
                double result = evaluator.Evaluate(input);

                ans = result;

                return result.ToString();
            }
            catch (Exception e)
            {
                return "Error evaluating expression: " + e.Message;
            }
        }

        // Main method runs the program in a loop until user types "exit"
        static void Main(string[] args)
        {
            string input;
            Console.WriteLine("Enter the expressions :");
            while ((input = Console.ReadLine()) != "exit")
            {

                Console.WriteLine(ProcessCommand(input));
                Console.WriteLine("Enter the expressions or type 'exit' to quit:");

            }
        }
    }
}
