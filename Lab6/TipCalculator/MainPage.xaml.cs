namespace TipCalculator;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
		CalculateBtn.IsEnabled = false;
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		double tip;
		double bill;

		if(Double.TryParse(BillAmount.Text, out bill) && Double.TryParse(TipAmount.Text, out tip))
		{
            double tipCalc = bill * tip;
			double totalCalc = tipCalc + bill;
            TipAmount.Text = tipCalc.ToString();
			TotalAmount.Text = totalCalc.ToString();
        }
	}

	private void OnTextChanged(object sender, TextChangedEventArgs e)
	{
		double tip;
		double bill;

        if (Double.TryParse(BillAmount.Text, out bill) && Double.TryParse(TipAmount.Text, out tip))
        {
			CalculateBtn.IsEnabled = true;
        }
		else
			CalculateBtn.IsEnabled = false;	

    }
}

