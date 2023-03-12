This script is a service class that provides methods for loading and instantiating assets from Addressable Assets, a Unity package that provides an alternative to the traditional asset loading system in Unity.
The methods included in the script are:

- LoadAssetFromAddressables: Loads an asset with a specific ID and returns it as an AddressableInstance that contains the loaded asset and the handle used to load it.
- InstantiateAssetFromAddressables: Instantiates an asset with a specific ID under a specified parent object.
- LoadCatalog: Loads a content catalog file for a specific asset. Content catalogs are JSON files that contain information about the assets available for download from a remote server.
