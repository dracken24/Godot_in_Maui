# Guide d'export APK Android depuis Godot 4.5.1

## Prérequis

1. **Godot 4.5.1** installé
2. **Android SDK** installé (via Android Studio ou SDK standalone)
3. **JDK 17 ou supérieur** installé
4. Votre projet Godot ouvert

## Étape 1 : Configurer le template Android dans Godot

### 1.1 Ouvrir les paramètres d'export
1. Dans Godot, allez dans **Project → Export**
2. Cliquez sur **Add...** si aucun preset Android n'existe
3. Sélectionnez **Android**

### 1.2 Configurer le template Android
1. Dans la section **Options**, vérifiez que le chemin vers le SDK Android est correct :
   - **Debug Keystore** : Laissez vide ou spécifiez un chemin si vous avez un keystore
   - **Export Format** : Sélectionnez **APK** (pas AAB pour le moment)

### 1.3 Installer les templates Android
1. Si vous voyez "Export templates missing", cliquez sur **Manage Export Templates**
2. Téléchargez les templates Android pour Godot 4.5.1
3. Attendez la fin du téléchargement

## Étape 2 : Configurer les paramètres du projet

### 2.1 Paramètres généraux du projet
1. Allez dans **Project → Project Settings**
2. Dans l'onglet **Application → Config**, configurez :
   - **Name** : Nom de votre jeu (ex: "My Godot Game")
   - **Version** : Version de votre application (ex: "1.0")
   - **Version Name** : Nom de version lisible (ex: "1.0.0")

### 2.2 Paramètres Android spécifiques
1. Dans **Project Settings**, allez dans **Export → Android**
2. Configurez les paramètres suivants :

#### Package
- **Package/App Name** : `com.company.mygodotgame` (IMPORTANT: utilisez des minuscules, pas d'espaces)
- **Version Code** : `1` (incrémentez à chaque nouvelle version)
- **Version Name** : `1.0.0`

#### Permissions
- Ajoutez les permissions nécessaires si votre jeu en a besoin
- Pour un jeu simple, vous pouvez laisser vide

#### Icons
- Configurez les icônes de l'application si vous le souhaitez
- Sinon, les icônes par défaut seront utilisées

#### Keystore (pour la signature)
- **Use Custom Build** : Décochez pour un build de debug
- Pour un build release, vous devrez créer un keystore (voir section suivante)

## Étape 3 : Créer un Keystore (pour Release, optionnel pour Debug)

### 3.1 Créer un keystore avec keytool (Java)
Ouvrez PowerShell ou CMD et exécutez :

```powershell
keytool -genkeypair -v -keystore mygame.keystore -alias mygame -keyalg RSA -keysize 2048 -validity 10000
```

Remplissez les informations demandées :
- Mot de passe du keystore (notez-le bien !)
- Mot de passe de la clé (peut être le même)
- Informations sur vous/entreprise

### 3.2 Configurer le keystore dans Godot
1. Dans **Project Settings → Export → Android**
2. Dans la section **Keystore** :
   - **Debug** : Laissez vide (Godot utilisera un keystore de debug par défaut)
   - **Release** : Spécifiez le chemin vers votre `mygame.keystore`
   - Entrez les mots de passe si nécessaire

## Étape 4 : Exporter l'APK

### 4.1 Vérifier la configuration
1. Retournez dans **Project → Export**
2. Sélectionnez le preset **Android**
3. Vérifiez que tous les paramètres sont corrects

### 4.2 Exporter
1. Cliquez sur **Export Project**
2. Choisissez un emplacement pour sauvegarder l'APK
3. Nommez-le : `MyGodotGame.apk`
4. Cliquez sur **Save**

### 4.3 Vérifier l'export
- L'APK devrait être créé à l'emplacement spécifié
- La taille devrait être d'au moins quelques Mo (pas quelques Ko)

## Étape 5 : Installer l'APK

### 5.1 Via ADB
```powershell
adb install -r "chemin\vers\MyGodotGame.apk"
```

### 5.2 Vérifier le nom du package
```powershell
adb shell pm list packages | Select-String -Pattern "godot"
```

Le nom du package devrait correspondre à ce que vous avez configuré dans **Package/App Name**.

## Dépannage

### Erreur : "Export templates missing"
- Allez dans **Project → Export → Manage Export Templates**
- Téléchargez les templates Android

### Erreur : "Android SDK path not configured"
- Installez Android Studio
- Configurez le chemin dans **Editor Settings → Export → Android**
- Le chemin devrait être : `C:\Users\<VotreNom>\AppData\Local\Android\Sdk`

### Erreur : "JDK not found"
- Installez JDK 17 ou supérieur
- Configurez JAVA_HOME dans les variables d'environnement Windows

### L'APK ne s'installe pas
- Vérifiez que l'émulateur/appareil a assez d'espace
- Vérifiez que l'APK n'est pas corrompu (taille > 1 Mo généralement)
- Essayez de désinstaller une ancienne version : `adb uninstall com.company.mygodotgame`

## Notes importantes

1. **Nom du package** : Utilisez uniquement des minuscules et des points (ex: `com.company.mygame`)
2. **Version Code** : Doit être un nombre entier qui augmente à chaque version
3. **Keystore Debug** : Godot génère automatiquement un keystore de debug, vous n'avez pas besoin d'en créer un pour tester
4. **Keystore Release** : Nécessaire uniquement pour publier sur Google Play Store

