/*
Copyright 2014 Google Inc

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Xml.Linq;
using DriveProxy.Utils;

namespace DriveProxy.Service
{
  internal class ParameterInfo : XElementObject
  {
    protected string _name = string.Empty;
    protected Type _type = typeof (void);

    public ParameterInfo()
    {
    }

    public ParameterInfo(string name, Type type)
    {
      try
      {
        _name = name;
        _type = type;
      }
      catch (Exception exception)
      {
        Log.Error(exception);
      }
    }

    public virtual string Name
    {
      get { return _name; }
      set { _name = value; }
    }

    public virtual Type Type
    {
      get { return _type; }
      set { _type = value; }
    }

    public override XElement ToXElement()
    {
      try
      {
        var xElement = new XElement("Parameter",
                                    new XAttribute("Name", Name),
                                    new XAttribute("Type", Type.ToString())
          );

        return xElement;
      }
      catch (Exception exception)
      {
        Log.Error(exception);

        return null;
      }
    }
  }
}
