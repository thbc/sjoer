The classes, `ConnectedInfoCategory` and `InjectedInfoCategory`, manage collections of `InfoItem` objects and handle data retrieval and processing for these items.

### Abstract Base Class: InfoCategory

Serves as a base class for different categories of information, managing collections of `InfoItem` objects and defining the structure for data retrieval and processing.

- **Properties**: `name`, `infoItems`, `dataType`, `displayArea`, `lastDTO` - name, a collection of `InfoItem` objects, the type of data `(AIS)` , the area of display `(HorizonPlane, SkyArea, HUD)` , and the most recently retrieved \[Data Transfer Object\]([Data Transfer Object (DTO)](../../DOCUMENTATION/Scripts&Functions/Data%20Transfer%20Object%20%28DTO%29.md)).
- Initializes the class with name, data type, display area, and aligner (which is a `Player` object).
- `Update()`: Retrieves new info items, updates existing items, and returns a list of updated `InfoItem` objects.
- `Tick()`: Iterates through each `InfoItem` to perform an update.
- `RetrieveInfoItems()`: Abstract method meant to be overridden in derived classes to define how info items are retrieved.
- `OnDestroy()`: Abstract method for cleanup which needs to be implemented in derived classes.

### ConnectedInfoCategory Class

Manages `InfoItem` objects whose data is fetched from a connected source via `DataRetriever`.

- **Additional Property**: Instance of `DataRetriever` to fetch data.
- Inherits from `InfoCategory` and initializes `dataRetriever`.
- `RetrieveInfoItems()`: Fetches DTOs from `dataRetriever` if connected and handles new info items based on the retrieved data.
- `HandleNewInfoItems(DTO dto)`: Processes new data and updates or adds `InfoItem` objects accordingly.

### InjectedInfoCategory Class

**Manages `InfoItem` objects whose data is provided by an injected function, allowing for more flexible data sources.**

- **Additional Property**: `InfoItemInjector`: A function delegate that returns a list of `InfoItem` objects.
    
- Inherits from `InfoCategory` and sets the `InfoItemInjector`.
- `RetrieveInfoItems()`: Retrieves new info items using the `InfoItemInjector` and handles them.
- `HandleNewInfoItem(InfoItem infoItem)`: Processes new or existing info items and updates the collection based on their state.

&nbsp;