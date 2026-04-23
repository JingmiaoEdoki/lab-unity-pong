✅ Critères d'acceptation

Quand le jeu démarre, un paddle est contrôlé automatiquement par l'ordinateur

L'IA suit la balle de manière réactive avec un délai de réaction

L'IA est battable par un joueur humain

Le joueur contrôle son paddle normalement (clavier W/S)

Le score fonctionne identiquement au mode 2 joueurs

Le projet compile sans erreur
💡 Indications techniques
Étudiez Paddle.cs + InputReaderSO.cs pour comprendre le pattern de mouvement
L'IA la plus simple : comparer ball.position.y avec paddle.position.y et appliquer une force
Pour l'imperfection : délai de réaction ou offset aléatoire
Les paramètres (vitesse, réactivité) sont idéalement dans un ScriptableObject
🎁 Bonus
Difficulté configurable dans l'Inspector (vitesse de réaction, précision)
Désactiver l'input du Joueur 2 quand l'IA est active
Comportement d'anticipation : l'IA vise là où la balle va être, pas là où elle est