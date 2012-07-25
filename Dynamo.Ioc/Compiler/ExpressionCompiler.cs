
// Let the registration supply the compiler/visitor ?
// How to register what ICompiler to use? Pass it in Compile method ? or just expect user to compile by them selves

// Make a test of using custom compiler using Transient/Container Expression Registration in Ioc.Extensions 

// Put CompileMode on Container and remove from every individual Registration ?

namespace Dynamo.Ioc.Compiler
{
	public class ExpressionCompiler : ICompiler
	{
		public void Compile(IResolver resolver)
		{
			// Create new instance each time instead?
			var visitor = new ExpressionCompilerVisitor();

			foreach (var registration in resolver.Index)
			{
				var compilableRegistration = registration as IExpressionRegistration;

				if (compilableRegistration != null)
				{
					var compiledExpression = visitor.Compile(compilableRegistration);

					if (compiledExpression != compilableRegistration.Expression)
					{
						compilableRegistration.Expression = compiledExpression;
					}
				}
			}
		}
	}
}
