# RollupField

Moteur de rollup configurable pour Dynamics 365 / Dataverse.
Un plugin recalcule des champs d'un enregistrement parent à partir de ses enfants,
selon une configuration stockée dans Dataverse.

## Compilation

Ouvrir Ksga.RollupConfigurationEngine.sln dans Visual Studio, restaurer les packages
NuGet, puis compiler. La clé de signature (ksgaConsulting.snk) n'est pas dans le dépôt :
générer la sienne avec `sn -k`.
