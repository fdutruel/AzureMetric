# Documentation des Endpoints API - GraphApiAudit

Cette documentation décrit les endpoints REST exposés par l’API GraphApiAudit pour l’audit, l’analyse et la gestion des quotas d’utilisation de Microsoft Graph API sur un tenant Microsoft 365.

---

## Endpoints de base

### 1. Récupérer les métriques d’appels API
- **GET** `/audit/api-calls`
- **Description** : Liste brute des appels API Graph collectés (volume, durée, endpoint, date).

### 2. Récupérer les statistiques d’utilisation par workload
- **GET** `/audit/workloads`
- **Description** : Statistiques d’utilisation par type de workload Microsoft 365 (Exchange, SharePoint, etc.).

### 3. Récupérer les logs d’audit AD
- **GET** `/audit/audit-events`
- **Description** : Logs d’audit des modifications d’objets Active Directory.

### 4. Récupérer l’usage des connecteurs API
- **GET** `/audit/api-connectors`
- **Description** : Statistiques d’utilisation des connecteurs personnalisés.

### 5. Récupérer la consommation des applications enregistrées
- **GET** `/audit/app-registrations`
- **Description** : Nombre d’appels API par application (App Registration).

---

## Endpoints avancés (quotas, analyse fine)

### 6. Appels API par utilisateur
- **GET** `/audit/user-api-calls`
- **Description** : Liste brute des appels API par utilisateur.

### 7. Appels API par application et endpoint
- **GET** `/audit/app-endpoint-api-calls`
- **Description** : Liste brute des appels API par application et endpoint.

### 8. Appels API par utilisateur et application
- **GET** `/audit/user-app-api-calls`
- **Description** : Liste brute des appels API par utilisateur et application.

### 9. Erreurs d’appels API
- **GET** `/audit/api-call-errors`
- **Description** : Liste brute des erreurs (4xx/5xx) lors des appels API.

### 10. Throttling (limitation de débit)
- **GET** `/audit/api-throttling`
- **Description** : Liste brute des cas de throttling (erreurs 429).

---

## Endpoints d’agrégation et d’analyse

### 11. Agrégation par application
- **GET** `/audit/app-registrations/aggregate-by-app?days=7`
- **Description** : Nombre total d’appels API par application sur la période (par défaut 7 jours).
- **Paramètres** :
    - `days` (optionnel, défaut 7) : nombre de jours à analyser.

### 12. Agrégation par utilisateur
- **GET** `/audit/user-api-calls/aggregate-by-user?days=7`
- **Description** : Nombre total d’appels API par utilisateur sur la période.

### 13. Top endpoints les plus utilisés
- **GET** `/audit/app-endpoint-api-calls/top-endpoints?top=5&days=7`
- **Description** : Les endpoints les plus sollicités sur la période.
- **Paramètres** :
    - `top` (optionnel, défaut 5) : nombre de endpoints à retourner.
    - `days` (optionnel, défaut 7) : période d’analyse.

### 14. Agrégation par utilisateur et application
- **GET** `/audit/user-app-api-calls/aggregate-by-user-app?days=7`
- **Description** : Nombre total d’appels API par couple utilisateur/application.

### 15. Agrégation des erreurs par application
- **GET** `/audit/api-call-errors/aggregate-by-app?days=7`
- **Description** : Nombre d’erreurs API par application.

### 16. Agrégation du throttling par application
- **GET** `/audit/api-throttling/aggregate-by-app?days=7`
- **Description** : Nombre de cas de throttling (429) par application.

### 17. Moyenne d’appels par application et endpoint
- **GET** `/audit/app-endpoint-api-calls/average-by-app-endpoint?days=7`
- **Description** : Moyenne d’appels par application et endpoint sur la période.

### 18. Utilisateurs à fort volume d’appels
- **GET** `/audit/user-api-calls/heavy-users?days=7&threshold=1000`
- **Description** : Liste des utilisateurs ayant dépassé un certain seuil d’appels API sur la période.
- **Paramètres** :
    - `days` (optionnel, défaut 7) : période d’analyse.
    - `threshold` (optionnel, défaut 1000) : seuil d’alerte.

---

## Utilisation

- Tous les endpoints retournent des données au format JSON.
- Les paramètres de période (`days`) permettent d’adapter la fenêtre d’analyse.
- Les endpoints d’agrégation sont idéaux pour alimenter des dashboards, des alertes, ou des modules de gestion de quotas côté frontend.

---

## Exemple d’appel (avec curl)

```bash
curl "https://votre-api/audit/app-registrations/aggregate-by-app?days=30"
```

---

Pour toute question ou besoin d’agrégation supplémentaire, contactez l’équipe projet.
