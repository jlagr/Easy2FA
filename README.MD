# Postmant 2 Factors Authentication authomation

## First request. In test section save the result into a variable

`pm.test("Status code is 200", function () {
  pm.response.to.have.status(200);
  const jsonData = pm.response.json();
  postman.setEnvironmentVariable("twoFactorAuthTokenId", jsonData.twoFactorAuthTokenId);
});`

## Secound request. Set the variable in body:
`{
    "twoFactorTokenId": "{{twoFactorAuthTokenId}}",
    "code": "891680"
}`

Then in Test option save the token in other variable:

`pm.test("Status code is 200", function () {
  pm.response.to.have.status(200);
  const jsonData = pm.response.json();
  postman.setEnvironmentVariable("jwtToken", jsonData.accessToken);
});`

## Now we can use the jwToken variable in the header request:
`Authorization = Bearer {{jwtToken}}`
