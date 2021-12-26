namespace graphqlServer.Schema.Authors
{
    /// <summary>
    /// A person who wrote or co-wrote one or more books
    /// </summary>
    public class Author
    {
        /// <summary>
        /// Author Id. It's a GUID
        /// </summary>
        /// <value></value>
        public string Id { get; set; } = default!;

        /// <summary>
        /// Author's firstname
        /// </summary>
        /// <value></value>
        public string? FirstName { get; set; }
        
        /// <summary>
        /// Author's surname
        /// </summary>
        /// <value></value>
        public string? SurnName { get; set; }

        /// <summary>
        /// Author's web site, if she has one
        /// </summary>
        /// <value></value>
        public string? WebSite { get; set; }

        /// <summary>
        /// Author's email
        /// </summary>
        /// <value></value>
        public string? Email { get; set; }
    }
}