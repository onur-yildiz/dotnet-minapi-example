# README

API Link: [**https://spcase-server.herokuapp.com/**](https://spcase-server.herokuapp.com/) (if you are going to make time consuming requests run this API locally, since heroku timeout is 30s.)\
[**App Repository**](https://github.com/onur-yildiz/spcase-app)\
[**App Deployment**](https://spcase-app.vercel.app/)

## Run

In the project directory run;

`dotnet run`

Deploys app on [http://localhost:5234](http://localhost:5234)\
You can use [Postman](https://www.postman.com/) to test, use the [App](https://spcase-app.vercel.app/) (Deployed on Vercel) or download its [source code](https://github.com/onur-yildiz/spcase-app) to run it locally.

&nbsp;
&nbsp;
&nbsp;

## Endpoints

This API has only one endpoint.

### **Statistics Summary**

Returns the summed up statistics between requested dates.

```perl
  GET /intra-day-trade-history-summary?startDate={START_DATE}&endDate={END_DATE} 
```

  `START_DATE` starting date of the interval. (`YYYY-MM-DD`)\
  `END_DATE` ending date of the interval. (`YYYY-MM-DD`)

### Response Body Example

  ```json
  [
      {
          "conract": "PH22040305",
          "totalTransactionFee": 5551.894,
          "totalTransactionAmount": 136,
          "weightedAveragePrice": 40.82275
      },
      {
          "conract": "PH22040303",
          "totalTransactionFee": 11805.516000000001,
          "totalTransactionAmount": 113,
          "weightedAveragePrice": 104.473592920354
      },
      {
          "conract": "PH22040304",
          "totalTransactionFee": 10201.482,
          "totalTransactionAmount": 135,
          "weightedAveragePrice": 75.56653333333334
      },
      {
          "conract": "PH22040306",
          "totalTransactionFee": 2759.332,
          "totalTransactionAmount": 61,
          "weightedAveragePrice": 45.23495081967213
      },
  ]
  ```
