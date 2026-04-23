🎯 Contexte
Un Pong noir et blanc ne séduit pas un enfant de 8 ans. L'objectif est de créer une ambiance "neon arcade" inspirée de Geometry Wars / Tron : des couleurs vives, des effets lumineux, des traînées, des particules.

✅ Critères d'acceptation

Chaque élément du jeu a une couleur néon distincte (balle cyan, paddle bleu/magenta, murs violet)

La balle laisse une traînée lumineuse (TrailRenderer)

Des particules colorées se déclenchent lors des impacts

Une explosion de particules plus spectaculaire se produit quand un but est marqué

L'ambiance est rehaussée par du post-processing (Bloom)

Le rendu est fluide (pas de chute de framerate perceptible)

Tous les effets sont générés par code — aucun asset graphique externe
💡 Indications techniques
Couleurs néon : utiliser le shader URP Lit standard avec émission activée (material.SetColor("_EmissionColor", color * intensity)) — pas besoin de shader custom
Bloom : configurer un Volume URP avec Bloom — les couleurs émissives le déclenchent automatiquement
TrailRenderer : time, startWidth, endWidth, material, colorGradient
ParticleSystem : MainModule, EmissionModule, ShapeModule
Les effets doivent s'intégrer via les événements existants (BallCollided, GoalHit, GameStarted)
💡 Suggestions UX
Pensez Tron, Geometry Wars, neon arcade — des couleurs qui "pètent" sur fond sombre
Couleurs : balle cyan, paddle joueur bleu, paddle adversaire magenta, murs violet sombre
🎁 Bonus
Screen shake léger à chaque collision avec un paddle
Couleur de la balle qui varie selon sa vitesse (bleu = lent → blanc = rapide)
Grille de fond animée style Tron (via LineRenderers)
Pulse sur les scores quand un point est marqué