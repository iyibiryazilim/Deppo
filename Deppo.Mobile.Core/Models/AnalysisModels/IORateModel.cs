using System;

namespace Deppo.Mobile.Core.Models.AnalysisModels;

public class IORateModel
{
    public string Label { get; }
    public double Value { get; }

    public IORateModel(string label, double value)
    {
        Label = label;
        Value = value;
    }
}
