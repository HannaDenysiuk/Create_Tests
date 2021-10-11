using System.Xml.Serialization;
namespace Xml2CSharp
{
    [XmlRoot(ElementName = "Answer")]
	public class Answer
	{
		[XmlElement(ElementName = "Description")]
		public string Description { get; set; }
		[XmlElement(ElementName = "IsRight")]
		public string IsRight { get; set; }
	}

}
