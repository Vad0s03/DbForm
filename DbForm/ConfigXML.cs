using System;
using System.Xml;

public class ConfigXML:IDisposable
{
	private XmlDocument? _xDoc= new();
	private XmlElement? _xRoot=null;

	private bool _disposed = false;
	
	public ConfigXML()
    {
        try
        {
            _xDoc.Load("..\\..\\config.xml");
            _xRoot = _xDoc.DocumentElement;
        }
        catch (Exception)
        {

        }

    }

    public void XmlWrite(string keyWord, string value)
    {
		XmlNode? attr = null;
		if (_xRoot != null)
		{
			foreach (XmlElement xmlElement in _xRoot)
			{
				attr = xmlElement.Attributes.GetNamedItem(keyWord);

			}

		}
		if (attr != null)
        {
			attr.Value = value;
		}
        else
        {
            if (_xDoc != null)
            {
				XmlElement personElem = _xDoc.CreateElement("Path");

				XmlAttribute nameAttr = _xDoc.CreateAttribute(keyWord);
				XmlText nameText = _xDoc.CreateTextNode(value);
				nameAttr.AppendChild(nameText);
				personElem.Attributes.Append(nameAttr);
				_xRoot?.AppendChild(personElem);
			}			
		}
			
	}
	public string XmlGetNamedItem(string word)
    {
		XmlNode? attr = null;
		if (_xRoot != null)
        {			
			foreach (XmlElement xmlElement in _xRoot)
			{
				attr = xmlElement.Attributes.GetNamedItem(word);

			}
			
		}
		if (attr == null || attr.Value == null)
        {
			return String.Empty;
		}
        else
        {
			return attr.Value;	
        }
		
	}

	public void Dispose()
    {
		Dispose(true);

		GC.SuppressFinalize(this);
	}

	protected virtual void Dispose(bool disposing)
    {
		if(_disposed)
        {
			return;
        }
        if (disposing)
        {

        }
        if (_xDoc != null)
        {
			_xDoc.Save("..\\..\\config.xml");
		}
		_xDoc=null;
		_xRoot=null;	
		_disposed = true;
	}
    ~ConfigXML()
    {
		Dispose(false);
    }
	
}
