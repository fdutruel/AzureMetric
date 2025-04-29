using System;

namespace GraphApiAudit.Api.Data.Entities
{
    /// <summary>
    /// Log d'audit d'une modification d'objet AD.
    /// </summary>
    public class AuditEvent
    {
        /// <summary>
        /// Identifiant unique de l'événement.
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Type d'objet modifié.
        /// </summary>
        public string ObjectType { get; set; }
        /// <summary>
        /// Identifiant de l'objet modifié.
        /// </summary>
        public string ObjectId { get; set; }
        /// <summary>
        /// Acteur ayant réalisé la modification.
        /// </summary>
        public string Actor { get; set; }
        /// <summary>
        /// Type de modification (création, suppression, etc.).
        /// </summary>
        public string ChangeType { get; set; }
        /// <summary>
        /// Date et heure de l'événement.
        /// </summary>
        public DateTime Timestamp { get; set; }
    }
}