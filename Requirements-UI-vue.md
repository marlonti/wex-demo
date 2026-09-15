# Summary 

Build a vue 3 UI application that interfaces with our api in (./api/ folder).

# Requirements 

## Requirement #1: Store a Purchase Transaction

The application must be able to accept and store (i.e., persist) a purchase transaction with a description, transaction date, and a purchase amount in United States dollars. The UI will send this request to our API, and store the results.

### Field requirements 
- Description: must not exceed 50 characters
- Transaction date: must be a valid date format 
- Purchase amount: must be a valid positive amount rounded to the nearest cent 

## Requirement #2: Provide a View to list all Purchase Transactions

Include aforementioned fields and include a link to view the additional details, which will be listed in Requirement #3.

## Requirement #3: Retrieve and Display a Purchase Transaction in a Specified Country’s Currency

Based upon purchase transactions previously submitted and stored, the application must send a country's currency to our api's get endpoint to retrieve the stored purchase transactions converted to the specified currency.

The retrieved purchase should include the identifier, the description, the transaction date, the original US dollar purchase amount, the exchange rate used, and the converted amount based upon the specified currency’s exchange rate for the date of the purchase.  
