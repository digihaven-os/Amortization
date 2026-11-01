using Amortization.Domain.Schedules;
using Amortization.Infrastructure.Export;
using Amortization.Infrastructure.Options;
using Amortization.Presentation;

const string optionsFilePath = "options.txt";

var options = LoanOptionsParser.Parse(optionsFilePath);

Console.WriteLine($"Loan Amount: {options.LoanAmount}");
Console.WriteLine($"Loan Term (months): {options.LoanTerm?.Months}");
Console.WriteLine($"Rate: {options.AnnualNominalRate}%");
Console.WriteLine($"Mortgage Insurance: {options.MortgageInsurance?.ToString() ?? "Not specified"}");
Console.WriteLine($"Start Date: {options.StartDate}");
Console.WriteLine();

var schedule = AmortizationSchedule.Generate(options);
var outputFilePath = options.OutputFilePath;

ScheduleConsolePrinter.Print(schedule);
ScheduleCsvWriter.Write(schedule, outputFilePath);

Console.WriteLine();
Console.WriteLine($"Amortization schedule saved to '{outputFilePath}'.");
