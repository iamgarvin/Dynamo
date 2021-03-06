﻿using System.Globalization;
using Autodesk.Revit.DB;
using Dynamo.Interfaces;
using Dynamo.Units;
using Dynamo.Utilities;
using Dynamo.ViewModels;

namespace Dynamo.Applications
{
    /// <summary>
    /// An Revit-specific implementation of IWatchHandler that is set on the Controller at startup.
    /// The main Process method dynamically dispatches to the appropriate
    /// internal method based on the type. For every time for which you would like
    /// to have a custom representation in the watch, you will need an additional
    /// method on this handler
    /// 
    /// NOTE:
    /// Many of these methods duplicate those found in the DefaultWatchHandler.
    /// As such, this class should extend DefaultWatchHandler. However, because the processsing
    /// methods are dynamically dispatched, it doesn't play nicely with inheritance and these
    /// methods have to be duplicated here.
    /// </summary>
    public class RevitWatchHandler : IWatchHandler
    {
        internal WatchItem ProcessThing(Element element, string tag, bool showRawData = true)
        {
            var id = element.Id;

            var node = new WatchItem(element.Name);
            node.Clicked += () => dynRevitSettings.Doc.ShowElements(element);
            node.Link = id.IntegerValue.ToString(CultureInfo.InvariantCulture);

            return node;
        }

        internal WatchItem ProcessThing(XYZ pt, string tag, bool showRawData = true)
        {
            var um = dynSettings.Controller.UnitsManager;

            if (!showRawData)
            {
                ///xyzs will be in feet, but we need to show them
                ///in the display units of choice
                /// 

                var xyzStr = string.Format("{0:f3}, {1:f3}, {2:f3}",
                    new Units.Length(pt.X / SIUnit.ToFoot, um),
                    new Units.Length(pt.Y / SIUnit.ToFoot, um),
                    new Units.Length(pt.Z / SIUnit.ToFoot, um));

                return new WatchItem("{" + xyzStr + "}", tag);
            }
            
            return new WatchItem(pt.ToString(), tag);
        }

        internal WatchItem ProcessThing(object value, string tag, bool showRawData = true)
        {
            var node = new WatchItem(value.ToString(), tag);
            return node;
        }

        internal WatchItem ProcessThing(SIUnit unit, string tag, bool showRawData = true)
        {
            if (showRawData)
                return new WatchItem(unit.Value.ToString(dynSettings.Controller.PreferenceSettings.NumberFormat, CultureInfo.InvariantCulture), tag);

            return new WatchItem(unit.ToString(), tag);
        }

        internal WatchItem ProcessThing(double value, string tag, bool showRawData = true)
        {
            return new WatchItem(value.ToString(dynSettings.Controller.PreferenceSettings.NumberFormat, CultureInfo.InvariantCulture), tag);
        }

        internal WatchItem ProcessThing(string value, string tag, bool showRawData = true)
        {
            return new WatchItem(value, tag);
        }

        public WatchItem Process(dynamic value, string tag, bool showRawData = true)
        {
            if(value == null)
                return new WatchItem("null");

            return ProcessThing(value, tag, showRawData);
        }
    }
}
