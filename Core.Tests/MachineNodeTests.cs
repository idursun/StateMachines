using NUnit.Framework;
using System.Linq;
using StateMachine.Core;

namespace StateMachine.Tests
{
    [TestFixture]
    public class MachineNodeTests
    {
         [Test]
         public void Test_Pin_Returns_a_Guid()
         {
             SampleNode sampleNode = new SampleNode();
             Pin pin = sampleNode.Pin<SampleNode>(x => x.Output1);
             Assert.That(pin, Is.Not.Null);
             Assert.That(pin.Name, Is.EqualTo("Output1"));
         }
        
        [Test]
         public void Test_Pin_Returns_same_Guid()
         {
             SampleNode sampleNode = new SampleNode();
             Pin pin1 = sampleNode.Pin<SampleNode>(x => x.Output1);
             Pin pin2 = sampleNode.Pin<SampleNode>(x => x.Output1);
             Assert.That(pin1, Is.EqualTo(pin2));
         }

         [Test]
         public void Test_Pin_Throws_If_expression_is_not_a_property()
         {
             SampleNode sampleNode = new SampleNode();
             Assert.That(delegate
             {
                 sampleNode.Pin<SampleNode>(x => x.GetData());
             }, Throws.Exception);
         }

        [Test]
        public void Test_GetPins_Returns_only_inputs()
        {
            SampleNode sampleNode = new SampleNode();
            var inputs = sampleNode.GetPins(PinType.Input);

            Assert.That(inputs, Is.Not.Null);
            Assert.That(inputs.Select(x => x.Name), Is.EquivalentTo(new[] {"Input1", "Input2"}));
        } 
        
        [Test]
        public void Test_GetPins_Returns_only_outputs()
        {
            SampleNode sampleNode = new SampleNode();
            var inputs = sampleNode.GetPins(PinType.Output);

            Assert.That(inputs, Is.Not.Null);
            Assert.That(inputs.Select(x => x.Name), Is.EquivalentTo(new[] {"Output1"}));
        } 
        
        [Test]
        public void Test_GetPins_Throws_If_pintype_is_none()
        {
            SampleNode sampleNode = new SampleNode();
            Assert.That(delegate
            {
                sampleNode.GetPins(PinType.None).ToList();
            }, Throws.Exception);
        }
    }

    public class SampleNode : Function
    {
        [Input]
        public string Input1 { get; set; }
        [Input]
        public string Input2 { get; set; }

        [Output]
        public string Output1 { get; set; }

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