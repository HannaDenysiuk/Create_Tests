using System;
using System.Xml.Serialization;
using System.Collections.Generic;
namespace Xml2CSharp
{

    [XmlRoot(ElementName = "Test")]
	public class Test
	{
		[XmlElement(ElementName = "Author")]
		public string Author { get; set; }
		[XmlElement(ElementName = "TestName")]
		public string TestName { get; set; }
		[XmlElement(ElementName = "QuestionCount")]
		public string QuestionCount { get; set; }
		[XmlElement(ElementName = "Question")]
		public List<Question> Question { get; set; }
	}

}
