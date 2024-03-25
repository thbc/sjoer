`token_url`: barentswatch url for generating a session token :`https://id.barentswatch.no/connect/token`

`ais_url`:  url for bwapi v2 for retrieving realtime vessel data for all vessels within a bounding box area (xMin > xMax, yMin > yMax) :

`https://www.barentswatch.no/bwapi/v2/geodata/ais/openpositions?Xmin={0}&Xmax={1}&Ymin={2}&Ymax={3}`

`auth_format`: additional string for authenticating and generating a token : `client_id={0}&scope=api&client_secret={1}&grant_type=client_credentials` 

`client_id`: encoded string containing the email address and client name (as registered in barentswatch.no) but with special characters extra encoded (@ = %40): `name%40email.com%3ClientName`

`client_secret`: your password/secret