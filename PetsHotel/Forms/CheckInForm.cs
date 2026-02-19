using PetsHotel.Models;

namespace PetsHotel.Forms;

public partial class CheckInForm : Form
{
    private readonly List<Dog> _dogs;

    public Dog? SelectedDog { get; private set; }
    public string Notes => txtNotes.Text.Trim();

    public CheckInForm(List<Dog> dogs)
    {
        _dogs = dogs;
        InitializeComponent();
        PopulateDogs();
    }

    private void PopulateDogs()
    {
        cmbDog.Items.Clear();
        foreach (var dog in _dogs)
            cmbDog.Items.Add(dog);

        if (cmbDog.Items.Count > 0)
            cmbDog.SelectedIndex = 0;
    }

    private void radExisting_CheckedChanged(object sender, EventArgs e)
    {
        pnlExisting.Visible = radExisting.Checked;
        pnlNewDog.Visible   = !radExisting.Checked;
    }

    private void btnCheckIn_Click(object sender, EventArgs e)
    {
        if (radExisting.Checked)
        {
            if (cmbDog.SelectedItem is not Dog dog)
            {
                MessageBox.Show("Please select a dog.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            SelectedDog = dog;
        }
        else
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter the dog's name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtBreed.Text))
            {
                MessageBox.Show("Please enter the dog's breed.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBreed.Focus();
                return;
            }
            if (!int.TryParse(txtAge.Text, out int age) || age < 0 || age > 30)
            {
                MessageBox.Show("Please enter a valid age (0–30).", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtAge.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtOwnerName.Text))
            {
                MessageBox.Show("Please enter the owner's name.", "Validation",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtOwnerName.Focus();
                return;
            }

            SelectedDog = new Dog
            {
                Name       = txtName.Text.Trim(),
                Breed      = txtBreed.Text.Trim(),
                Age        = age,
                OwnerName  = txtOwnerName.Text.Trim(),
                OwnerPhone = txtOwnerPhone.Text.Trim()
            };
        }

        DialogResult = DialogResult.OK;
        Close();
    }

    private void btnCancel_Click(object sender, EventArgs e)
    {
        DialogResult = DialogResult.Cancel;
        Close();
    }
}
