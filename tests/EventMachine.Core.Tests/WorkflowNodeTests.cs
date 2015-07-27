using NUnit.Framework;
using System.Linq;
using EventMachine.Core.Utils;

namespace EventMachine.Core.Tests
{
    [TestFixture]
    public class WorkflowNodeTests
    {
         [Test]
         public void Test_Pin_ReturnsNotNull()
         {
             SampleNode sampleNode = new SampleNode();
             Pin pin = sampleNode.Pin(x => x.Output1);
             Assert.That(pin, Is.Not.Null);
             Assert.That(pin.Name, Is.EqualTo("Output1"));
         }
        
        [Test]
         public void Test_Pin_ByNameReturnsNotNull()
         {
             SampleNode sampleNode = new SampleNode();
             Pin pin = sampleNode.Pin("Output1");
             Assert.That(pin, Is.Not.Null);
             Assert.That(pin.Name, Is.EqualTo("Output1"));
         }

        [Test]
         public void Test_Pin_ByName_From_BaseTypeReturnsNotNull_()
         {
             WorkflowNode sampleNode = new SampleNode();
             Pin pin = sampleNode.Pin("Output1");
             Assert.That(pin, Is.Not.Null);
             Assert.That(pin.Name, Is.EqualTo("Output1"));
         }
        
         [Test]
         public void Test_Pin_Throws_IfExpressionIsNotAProperty()
         {
             SampleNode sampleNode = new SampleNode();
             Assert.That(delegate
             {
                 sampleNode.Pin(x => x.GetData());
             }, Throws.Exception);
         }

        [Test]
        public void Test_GetPins_ReturnsOnlyInputs()
        {
            SampleNode sampleNode = new SampleNode();
            var inputs = sampleNode.GetPins(PinType.Input);

            Assert.That(inputs, Is.Not.Null);
            Assert.That(inputs.Select(x => x.Name), Is.EquivalentTo(new[] {"Input1", "Input2"}));
        } 
        
        [Test]
        public void Test_GetPins_ReturnsOnlyOutputs()
        {
            SampleNode sampleNode = new SampleNode();
            var inputs = sampleNode.GetPins(PinType.Output);

            Assert.That(inputs, Is.Not.Null);
            Assert.That(inputs.Select(x => x.Name), Is.EquivalentTo(new[] {"Output1"}));
        } 

        [Test]
        public void Test_GetPins_ReturnsOnlyExecute()
        {
            SampleNode sampleNode = new SampleNode();
            var inputs = sampleNode.GetPins(PinType.Execute);

            Assert.That(inputs, Is.Not.Null);
            Assert.That(inputs.Select(x => x.Name), Is.EquivalentTo(new[] {"Exec1", "Exec2"}));
        } 
        
        [Test]
        public void Test_GetPins_ThrowsIfPintypeIsNone()
        {
            SampleNode sampleNode = new SampleNode();
            Assert.That(delegate
            {
                sampleNode.GetPins(PinType.None).ToList();
            }, Throws.Exception);
        }
    }

    public class SampleNode : WorkflowFunction
    {
        [Input]
        public string Input1 { get; set; }
        [Input]
        public string Input2 { get; set; }

        [Output]
        public string Output1 { get; set; }

        public IExecutable Exec1 { get; set; }
        public IExecutable Exec2 { get; set; }

        public string GetData()
        {
            return string.Empty;
        }

        public override void Evaluate()
        {
            Output1 = "Hello";
        }
    }
}