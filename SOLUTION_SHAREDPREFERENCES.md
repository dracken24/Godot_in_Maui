# Solution : SharedPreferences (équivalent localStorage Android)

## Réponse à votre question

**OUI**, Android a un équivalent de `localStorage` : **SharedPreferences** !

C'est un système de stockage clé-valeur persistant, exactement comme `localStorage` dans les navigateurs web.

## Problème avec le partage entre apps

**IMPORTANT** : `SharedPreferences` avec `MODE_PRIVATE` ne peut **PAS** être partagé entre deux apps différentes (MAUI et Godot).

Chaque app a son propre espace de SharedPreferences :
- MAUI : `/data/data/com.companyname.platformgame/shared_prefs/`
- Godot : `/data/data/com.company.mygodotgame/shared_prefs/`

## Solutions possibles

### ✅ Solution 1 : Intent Extras (RECOMMANDÉ - déjà implémenté)

**Avantages** :
- ✅ Fonctionne automatiquement
- ✅ Pas besoin de permissions
- ✅ Pas de problème de partage

**Comment ça fonctionne** :
- MAUI passe l'email via `Intent.PutExtra("user_email", email)`
- Godot lit via le plugin `IntentExtrasPlugin`

### ✅ Solution 2 : Fichier XML partagé (Alternative)

Créer un fichier XML dans un emplacement accessible par les deux apps.

**Avantages** :
- ✅ Similaire à SharedPreferences (format XML)
- ✅ Accessible par les deux apps

**Inconvénients** :
- ⚠️ Nécessite des permissions de stockage
- ⚠️ Problèmes avec Scoped Storage sur Android 10+

### ⚠️ Solution 3 : MODE_WORLD_READABLE (Déprécié)

Utiliser `MODE_WORLD_READABLE` pour partager SharedPreferences.

**Problèmes** :
- ❌ Déprécié depuis Android 7.0
- ❌ Ne fonctionne plus sur Android 10+
- ❌ Sécurité compromise

### ✅ Solution 4 : ContentProvider (Recommandé pour production)

Créer un ContentProvider dans l'app MAUI pour exposer les données.

**Avantages** :
- ✅ Solution moderne et sécurisée
- ✅ Fonctionne sur toutes les versions d'Android
- ✅ Contrôle d'accès granulaire

**Inconvénients** :
- ⚠️ Plus complexe à implémenter

## Solution actuelle (implémentée)

Nous avons implémenté **deux solutions en parallèle** :

1. **Intent Extras** (PRIORITÉ 1) - Fonctionne comme argc/argv
2. **SharedPreferences** (PRIORITÉ 2) - Équivalent localStorage (mais pas partagé entre apps)

### Pourquoi SharedPreferences ne fonctionne pas entre apps ?

```java
// MAUI écrit dans :
/data/data/com.companyname.platformgame/shared_prefs/platformgame_shared_prefs.xml

// Godot lit dans :
/data/data/com.company.mygodotgame/shared_prefs/platformgame_shared_prefs.xml

// ❌ Ce sont deux fichiers différents !
```

## Recommandation finale

**Utilisez Intent Extras** (déjà implémenté) car :
- ✅ Fonctionne automatiquement
- ✅ Pas de problème de partage
- ✅ Pas besoin de permissions
- ✅ Équivalent à argc/argv (comme vous l'avez demandé)

**SharedPreferences** est utile pour stocker des données **dans la même app**, mais pas pour partager entre apps.

## Code implémenté

### MAUI (écriture)
```csharp
// Écrit dans SharedPreferences (pour usage interne MAUI)
WriteToSharedPreferences(email);

// Passe aussi via Intent extras (pour Godot)
intent.PutExtra("user_email", email);
```

### Godot (lecture)
```csharp
// PRIORITÉ 1: SharedPreferences (si disponible)
// PRIORITÉ 2: Intent Extras (fonctionne toujours)
// PRIORITÉ 3: Fichier de configuration
```

## Conclusion

**SharedPreferences est l'équivalent de localStorage**, mais il ne peut pas être partagé entre deux apps différentes avec `MODE_PRIVATE`.

**La meilleure solution est Intent Extras** (déjà implémentée), qui fonctionne comme argc/argv et ne nécessite pas de partage de fichiers.

