using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckOutUI : MonoBehaviour
{
    public GameObject cashPanel; // Panel for cash payments
    public GameObject cardPanel; // Panel for card payments
    public GameObject changePanel; // Panel for returning change

    public Text totalCostText; // Text displaying the total cost
    public Text changeText; // Text displaying change to be returned

    // Cash payment UI
    public Button cashCompleteButton; // Button to complete cash payment
    public Text cashTotalCostText; // Text displaying the total cost in cash panel
    public Text cardTotalCostText; // Text displaying the total cost in card panel

    // Card payment UI
    public InputField cardAmountInput; // Input field for card amount
    public Button confirmCardButton; // Button to confirm card payment
    public Text paymentStatusText; // Feedback text

    // Change UI
    public InputField changeInputField; // Input field for returning change
    public Button returnChangeButton; // Button to return the change

    // Additional payment UI

    private float totalCosttext; // Stores the total cost for processing
    private float changeToReturn; // Stores the change to return
    private string selectedPaymentMethod;

    public Button[] keypadButtons; // Array of keypad buttons (0-9)
    public Button enterButton; // Enter button
    public Button clearButton; // Clear button

    private string enteredAmount = ""; // To store the entered amount
    private float totalCost;


    void Start()
    {
        foreach (Button button in keypadButtons)
        {
            button.onClick.AddListener(() => OnKeypadButtonPress(button.GetComponentInChildren<Text>().text));
        }

        // Assign the Clear and Enter button functionality
        clearButton.onClick.AddListener(ClearInput);
        enterButton.onClick.AddListener(ConfirmCardPayment);
        // Initially hide all panels
        cashPanel.SetActive(false);
        cardPanel.SetActive(false);
        changePanel.SetActive(false);

        // Assign button click listeners
        cashCompleteButton.onClick.AddListener(() => ProcessCashPayment());
        confirmCardButton.onClick.AddListener(() => ConfirmCardPayment());
        returnChangeButton.onClick.AddListener(() => ReturnChange());

        // Add listener for Enter key on the card amount input field
        cardAmountInput.onEndEdit.AddListener(HandleCardInputEndEdit);
    }

    void Update()
    {
        // Additional update logic can be added here if necessary
    }
    public void OnKeypadButtonPress(string buttonValue)
    {
        enteredAmount += buttonValue;
        cardAmountInput.text = enteredAmount; // Update the input field with the entered amount
    }

    // Clears the input field and resets the entered amount
    public void ClearInput()
    {
        enteredAmount = "";
        cardAmountInput.text = ""; // Reset the input field
    }
    public void DisplayCheckout(float totalCost, string paymentMethod)
    {
        this.totalCost = totalCost;
        selectedPaymentMethod = paymentMethod;

        // Display the total cost in both panels
        cashTotalCostText.text = "Total: $" + totalCost.ToString("F2");
        cardTotalCostText.text = "Total: $" + totalCost.ToString("F2");

        // Show the correct panel based on the payment method
        if (paymentMethod == "Cash")
        {
            ShowCashPanel();
        }
        else if (paymentMethod == "Card")
        {
            ShowCardPanel();
        }
    }

    // Shows the cash payment panel and hides the card panel
    private void ShowCashPanel()
    {
        cashPanel.SetActive(true);
        cardPanel.SetActive(false);
        paymentStatusText.text = ""; // Reset payment status text
    }

    // Shows the card payment panel and hides the cash panel
    private void ShowCardPanel()
    {
        cardPanel.SetActive(true);
        cashPanel.SetActive(false);
        paymentStatusText.text = ""; // Reset payment status text
    }

    // Process cash payment
    private void ProcessCashPayment()
    {
        Debug.Log("Cash payment processed.");
        CompletePayment("Cash");
    }

    // Confirm card payment after entering the amount
    private void ConfirmCardPayment()
    {
        float enteredAmount = 0f;

        // Try to parse the entered amount
        if (float.TryParse(cardAmountInput.text, out enteredAmount))
        {
            if (enteredAmount == totalCost)
            {
                Debug.Log("Card payment processed.");
                CompletePayment("Card");
            }
            else
            {
                Debug.LogWarning("Incorrect card payment amount.");
                paymentStatusText.text = "Please enter the exact amount: $" + totalCost.ToString("F2");
            }
        }
        else
        {
            Debug.LogWarning("Invalid card amount entered.");
            paymentStatusText.text = "Invalid amount. Please enter a valid number.";
        }
    }

    // This method will handle the "Enter" key press after editing the card amount input field
    private void HandleCardInputEndEdit(string input)
    {
        if (Input.GetKeyDown(KeyCode.Return)) // Check if the Enter key is pressed
        {
            ConfirmCardPayment(); // Call the card payment confirmation
        }
    }

    // Show the UI for returning change
    public void DisplayChangeUI(float changeAmount)
    {
        changeToReturn = changeAmount;
        changeText.text = "You need to return: $" + changeAmount.ToString("F2");
        changePanel.SetActive(true);
        cardPanel.SetActive(false);
        cashPanel.SetActive(false);
    }

    // Process returning the change
    private void ReturnChange()
    {
        float enteredAmount = 0f;

        // Try to parse the entered amount for returning change
        if (float.TryParse(changeInputField.text, out enteredAmount))
        {
            if (Mathf.Approximately(enteredAmount, changeToReturn))
            {
                Debug.Log("Change returned correctly.");
                CompletePayment("Cash with correct change");
            }
            else
            {
                Debug.LogWarning("Incorrect change returned.");
                paymentStatusText.text = "Incorrect amount. You need to return exactly: $" + changeToReturn.ToString("F2");
            }
        }
        else
        {
            Debug.LogWarning("Invalid change amount entered.");
            paymentStatusText.text = "Invalid amount. Please enter a valid number.";
        }
    }

    // Process the additional payment
    private void RequestAdditionalPayment()
    {
        Debug.Log("Requesting additional payment from AI.");
        CompletePayment("Cash with additional payment");
    }

    // Complete the payment process
    private void CompletePayment(string method)
    {
        // Close all panels
        cashPanel.SetActive(false);
        cardPanel.SetActive(false);
        changePanel.SetActive(false);

        // Provide feedback (optional)
        paymentStatusText.text = method + " payment successful!";
        ClearInput();
    }
}
