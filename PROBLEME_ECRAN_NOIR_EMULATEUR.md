# Problème d'écran noir sur l'émulateur Android avec Godot

## Problème identifié

Le jeu Godot fonctionne correctement sur un appareil physique mais affiche un écran noir sur l'émulateur Android. C'est un problème courant lié au rendu OpenGL/GPU dans les émulateurs.

## Causes possibles

1. **Problème de rendu OpenGL ES** : Les émulateurs Android ont souvent des problèmes avec le rendu OpenGL, surtout pour les jeux
2. **Accélération GPU manquante ou mal configurée** : L'émulateur peut ne pas avoir d'accélération GPU correctement configurée
3. **Version OpenGL incompatible** : L'émulateur peut utiliser une version d'OpenGL différente de celle attendue par Godot
4. **Limitations de l'émulateur** : Les émulateurs ne sont pas optimisés pour les jeux 3D

## Solutions

### Solution 1 : Configurer l'accélération GPU dans l'émulateur

1. Ouvrez **Android Virtual Device (AVD) Manager** dans Android Studio
2. Cliquez sur l'icône **Edit** (crayon) de votre émulateur
3. Cliquez sur **Show Advanced Settings**
4. Dans **Graphics**, sélectionnez :
   - **Hardware - GLES 2.0** (recommandé)
   - Ou **Automatic** (laisse Android choisir)
5. Cliquez sur **Finish**
6. Redémarrez l'émulateur

### Solution 2 : Utiliser un émulateur avec accélération GPU

Créez un nouvel émulateur avec :
- **API Level** : 30 ou supérieur
- **Graphics** : Hardware - GLES 2.0
- **RAM** : Au moins 2 GB
- **VM heap** : 512 MB ou plus

### Solution 3 : Configurer Godot pour l'émulateur

Dans votre projet Godot :

1. Allez dans **Project → Project Settings**
2. Cherchez **Rendering → Driver**
3. Essayez de changer le driver de rendu :
   - **Vulkan** → **OpenGL ES 3.0**
   - Ou **OpenGL ES 3.0** → **OpenGL ES 2.0** (plus compatible avec les émulateurs)

### Solution 4 : Exporter Godot avec des paramètres compatibles

Lors de l'export depuis Godot :

1. **Project → Export → Android**
2. Dans l'onglet **Options**, section **Graphics** :
   - Désactivez **Use Vulkan** si activé
   - Utilisez **OpenGL ES 2.0** ou **OpenGL ES 3.0**
3. Réexportez l'APK

### Solution 5 : Vérifier les logs Android

Capturez les logs pour voir les erreurs de rendu :

```powershell
# Voir les logs OpenGL/EGL
adb logcat | Select-String -Pattern "EGL|OpenGL|GLES|rendering|graphics"

# Voir les erreurs spécifiques à Godot
adb logcat | Select-String -Pattern "godot|Godot|FATAL"
```

### Solution 6 : Tester sur un appareil physique (recommandé)

Les émulateurs ne sont pas optimisés pour les jeux. Pour un développement de jeu, il est **fortement recommandé** de tester sur un appareil physique :

**Avantages d'un appareil physique :**
- Performance réelle
- Rendu GPU correct
- Pas de problèmes d'OpenGL
- Test des contrôles tactiles réels
- Test de la batterie et des performances

## Comportement normal

**C'est normal que :**
- L'app MAUI passe en arrière-plan quand Godot démarre (comportement Android standard)
- L'app MAUI reste en mémoire et peut être relancée via le gestionnaire de tâches
- Le processus de débogage s'arrête quand l'app passe en arrière-plan (Visual Studio)

**Ce n'est PAS normal que :**
- L'app MAUI se ferme complètement (elle devrait rester en mémoire)
- L'écran reste noir sur un appareil physique (indique un problème avec l'export Godot)

## Recommandations

1. **Pour le développement** : Utilisez un appareil physique Android pour tester les jeux
2. **Pour les tests rapides** : Utilisez l'émulateur mais acceptez que certains jeux ne fonctionnent pas correctement
3. **Pour la production** : Testez toujours sur plusieurs appareils physiques avant de publier

## Vérification

Pour vérifier si le problème vient de l'émulateur ou de l'export :

1. Testez l'APK Godot directement sur l'émulateur (sans passer par MAUI)
2. Si ça fonctionne directement mais pas via MAUI → problème de lancement depuis MAUI
3. Si ça ne fonctionne pas directement non plus → problème d'export Godot ou d'émulateur

## Note importante

Le fait que le jeu fonctionne sur un appareil physique mais pas sur l'émulateur est **normal et attendu** pour beaucoup de jeux utilisant OpenGL/Vulkan. Les émulateurs Android ne sont pas conçus pour les jeux 3D complexes.

