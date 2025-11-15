# Correction du problème d'export Godot - Aucune activité trouvée

## Problème identifié

L'APK Godot est installé mais Android ne trouve aucune activité déclarée. Cela signifie que l'export depuis Godot n'a pas correctement généré le fichier `AndroidManifest.xml` ou que l'activité principale n'est pas configurée.

## Solution : Réexporter correctement depuis Godot 4.5.1

### Étape 1 : Vérifier la configuration du projet

1. **Ouvrez votre projet dans Godot 4.5.1**
2. Allez dans **Project → Project Settings**
3. Vérifiez que **Application → Run → Main Scene** est défini avec une scène valide

### Étape 2 : Configurer l'export Android

1. **Project → Export**
2. Si aucun preset Android n'existe, cliquez sur **Add...** et sélectionnez **Android**
3. Sélectionnez le preset **Android**

### Étape 3 : Vérifier les paramètres d'export (CRITIQUE)

Dans l'onglet **Options**, section **Package** :

#### Paramètres obligatoires :
- **Package/App Name** : `com.company.mygodotgame` (en minuscules, sans espaces)
- **Version Code** : `1` (doit être un nombre entier)
- **Version Name** : `1.0.0`
- **Minimum SDK** : `21` (Android 5.0) ou supérieur
- **Target SDK** : `34` ou la dernière version disponible

#### Vérifications importantes :
- **Export Format** : Doit être **APK** (pas AAB)
- **Gradle Build** : Standard (32 bits) ou Standard (64 bits) selon votre cible

### Étape 4 : Vérifier les templates Android

1. Si vous voyez "Export templates missing" :
   - Cliquez sur **Manage Export Templates**
   - Téléchargez les templates Android pour Godot 4.5.1
   - Attendez la fin du téléchargement

### Étape 5 : Exporter l'APK

1. Cliquez sur **Export Project**
2. Choisissez un emplacement (ex: `Godot\MyGodotGame.apk`)
3. Cliquez sur **Save**
4. Attendez la fin de l'export (peut prendre plusieurs minutes)

### Étape 6 : Vérifier l'APK exporté

Vérifiez que :
- Le fichier existe et a une taille raisonnable (> 1 Mo généralement)
- L'export s'est terminé sans erreur

### Étape 7 : Réinstaller l'APK

```powershell
# Désinstaller l'ancienne version
adb uninstall com.company.mygodotgame

# Installer la nouvelle version
adb install -r "Godot\MyGodotGame.apk"

# Vérifier que l'app est installée
adb shell pm list packages | Select-String -Pattern "godot"
```

### Étape 8 : Vérifier les activités déclarées

```powershell
# Vérifier les activités
adb shell cmd package query-activities --brief com.company.mygodotgame
```

Si cette commande retourne des activités, l'export est correct. Sinon, il y a encore un problème avec l'export.

## Problèmes courants et solutions

### Problème 1 : "Export templates missing"
**Solution** : Téléchargez les templates via **Manage Export Templates**

### Problème 2 : L'APK s'exporte mais n'a pas d'activités
**Causes possibles** :
- La Main Scene n'est pas définie dans Project Settings
- Les templates Android sont corrompus
- L'export a été interrompu

**Solutions** :
1. Vérifiez que **Project → Project Settings → Application → Run → Main Scene** est défini
2. Réinstallez les templates Android
3. Réexportez l'APK

### Problème 3 : L'app crash au démarrage
**Causes possibles** :
- Ressources manquantes
- Permissions manquantes
- Problème avec la scène principale

**Solutions** :
1. Vérifiez les logs Android : `adb logcat | Select-String -Pattern "godot|FATAL|AndroidRuntime"`
2. Testez avec un projet Godot minimal (une scène avec juste un Label)
3. Vérifiez que toutes les ressources sont incluses dans l'export

## Test avec un projet minimal

Pour isoler le problème, créez un projet Godot minimal :

1. Créez un nouveau projet Godot
2. Créez une scène avec juste un Label "Hello World"
3. Définissez cette scène comme Main Scene
4. Exportez en APK
5. Testez si cet APK minimal fonctionne

Si l'APK minimal fonctionne, le problème vient de votre projet principal.

## Commandes utiles

```powershell
# Vérifier les packages installés
adb shell pm list packages | Select-String -Pattern "godot"

# Vérifier les activités d'un package
adb shell cmd package query-activities --brief com.company.mygodotgame

# Voir les informations détaillées du package
adb shell dumpsys package com.company.mygodotgame | Select-String -Pattern "Activity|activity"

# Lancer l'app manuellement (si l'activité est connue)
adb shell am start -n com.company.mygodotgame/com.godot.game.GodotApp

# Voir les logs en temps réel
adb logcat | Select-String -Pattern "godot|Godot|FATAL|AndroidRuntime"
```

## Après correction

Une fois l'APK correctement exporté et installé, votre code MAUI devrait pouvoir le lancer sans problème. Le code dans `MainPage.xaml.cs` essaiera automatiquement plusieurs méthodes pour lancer l'app.

