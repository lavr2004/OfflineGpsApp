using SQLite;

namespace OfflineGpsApp.CodeBase.Services.DatabaseService.Models;

//ADDED: for working with SQLite database and monuments table
[SQLite.Table("monuments")] //ADDED: explicitly map to table 'monuments'
public class DatabaseServiceMonumentModel
{
    [SQLite.PrimaryKey]
    public System.Int32 Id { get; set; } //ADDED: maps to 'id INTEGER PRIMARY KEY'
    public System.Int32? SourceId { get; set; } //ADDED: maps to 'source_id INTEGER'
    public System.Int32? ImageBaseUrlId { get; set; } //ADDED: maps to 'image_base_url_id INTEGER'
    public System.String ImageFilename { get; set; } //ADDED: maps to 'image_filename TEXT'
    public System.Int32? SourceBaseUrlId { get; set; } //ADDED: maps to 'source_base_url_id INTEGER'
    public System.String SourceIdentifier { get; set; } //ADDED: maps to 'source_identifier TEXT'
    public System.String Name { get; set; } //ADDED: maps to 'name TEXT NOT NULL'
    public System.Int32? ChronologyId { get; set; } //ADDED: maps to 'chronology_id INTEGER'
    public System.String ArchitecturalStyle { get; set; } //ADDED: maps to 'architectural_style TEXT'
    public System.Int32? MaterialId { get; set; } //ADDED: maps to 'material_id INTEGER'
    public System.Int32? FunctionId { get; set; } //ADDED: maps to 'function_id INTEGER'
    public System.String Description { get; set; } //ADDED: maps to 'description TEXT'
    public System.Int32? LocationPrecisionId { get; set; } //ADDED: maps to 'location_precision_id INTEGER'
    public System.DateTime? EntryDate { get; set; } //ADDED: maps to 'entry_date DATE'
    public System.Int32? VoivodeshipId { get; set; } //ADDED: maps to 'voivodeship_id INTEGER'
    public System.Int32? LocalityId { get; set; } //ADDED: maps to 'locality_id INTEGER'
    public System.String Street { get; set; } //ADDED: maps to 'street TEXT'
    public System.String AddressNumber { get; set; } //ADDED: maps to 'address_number TEXT'
    public System.Double Longitude { get; set; } //ADDED: maps to 'longitude REAL'
    public System.Double Latitude { get; set; } //ADDED: maps to 'latitude REAL'
    public System.Int32? ProtectionTypeId { get; set; } //ADDED: maps to 'protection_type_id INTEGER'
    public System.String InspireId { get; set; } //ADDED: maps to 'inspire_id TEXT'
}

