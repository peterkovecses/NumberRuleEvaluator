using NumberRuleEvaluator.Core.Configuration;
using NumberRuleEvaluator.Printing;
using NumberRuleEvaluator.Sample;
using CoreEvaluator = NumberRuleEvaluator.Core.Evaluation.NumberRuleEvaluator;

// Demonstrates the documented public API: configure a range, divisor rules, and a separator, then
// evaluate a number and forward the result to a console presentation adapter.
var configuration = new NumberRuleEvaluatorConfig(
    range: new NumberRange(14, 72),
    rules:
    [
        new DivisorRule(3, "Peter"),
        new DivisorRule(5, "Jeffrey")
    ],
    separator: " ");

var evaluator = new CoreEvaluator(configuration);
var coordinator = new NumberRulePrintCoordinator(evaluator, new ConsolePrinter());

coordinator.EvaluateAndPrint(15); // Prints "Jeffrey Peter"

