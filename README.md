Build a simple REST API with an endpoint that will calculate the taxes and perform currency exchange for invoices. 

The endpoint should take three inputs: 
	Invoice Date, 
	Invoice Pre-Tax Amount in Euro (EUR), and 
	Payment Currency. 
	
After completing currency calculations, the endpoint should return four outputs: 
	Pre-Tax Amount, 
	Tax Amount, 
	Grand Total, 
	and Exchange Rate.

Only two currencies need to be supported for currency conversion: 
	Canadian Dollar (CAD) and 
	US Dollar (USD). 

The exchange rates should be obtained from an external API. 
The exchange rate used for each calculation should be determined by the invoice date.

Tax is determined by the payment currency as follows. 
	CAD = 11%, 
	USD = 10%, and 
	EUR = 9%.


1.	Input: 
Invoice Date: Aug 5, 2020, 
Pre-Tax Amount: 123.45 EUR, 
Payment Currency: USD

Output : 
	Pre-Tax Total: 146.57 USD, 
	Tax Amount: 14.66 USD, 
	Grand Total: 161.23 USD, 
	Exchange Rate: 1.187247


2.	Input: 
Invoice Date: Jul 12, 2019, 
Pre-Tax Amount: 1,000.00 EUR, 
Payment Currency: EUR

Output: 
	Pre-Tax Total: 1,000.00 EUR, 
	Tax Amount: 90.00 EUR, 
	Grand Total: 1,090.00 EUR, 
	Exchange Rate: 1



3.	Input: 
Invoice Date: Aug 19, 2020, 
Pre-Tax Amount: 6,543.21 EUR, 
Payment Currency: CAD

Output: 
	Pre-Tax Total: 10,239.07 CAD, 
	Tax Amount: 1,126.30 CAD, 
	Grand Total: 11,365.37 CAD, 
	Exchange Rate: 1.564839
