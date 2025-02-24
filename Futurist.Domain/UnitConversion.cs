namespace Futurist.Domain;
public class UnitConversion
{
    public int RecId { get; set; }
    public decimal Factor { get; set; }
    public decimal Numerator { get; set; }
    public decimal Denominator { get; set; }
    public string FromUnit { get; set; } = string.Empty;
    public string ToUnit { get; set; } = string.Empty;
}
