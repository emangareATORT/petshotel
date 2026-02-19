using PetsHotel.Models;

namespace PetsHotel.Forms;

public partial class CheckOutForm : Form
{
    private readonly Stay _stay;

    public string Notes => txtNotes.Text.Trim();

    public CheckOutForm(Stay stay)
    {
        _stay = stay;
        InitializeComponent();
        PopulateStayInfo();
    }

    private void PopulateStayInfo()
    {
        lblDogValue.Text      = $"{_stay.DogName} ({_stay.Breed})";
        lblOwnerValue.Text    = $"{_stay.OwnerName}  {_stay.OwnerPhone}".Trim();
        lblCheckInValue.Text  = _stay.CheckInDate.ToString("dd MMM yyyy HH:mm");
        lblDaysValue.Text     = _stay.DaysStayed == 1 ? "1 day" : $"{_stay.DaysStayed} days";
        txtNotes.Text         = _stay.Notes;
    }

    private void btnCheckOut_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
