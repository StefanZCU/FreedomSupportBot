
<h1 align="center">FreedomSupportBot</h1>

<p align="center">
  <strong>AI-powered customer support chatbot</strong><br/>
  Built with C#, ASP.NET Core, EF Core, Telegram Bot API & OpenAI
</p>

<p align="center">
  <img src="https://img.shields.io/badge/.NET-ASP.NET%20Core-blueviolet" />
  <img src="https://img.shields.io/badge/Language-C%23-239120" />
  <img src="https://img.shields.io/badge/Database-SQL%20Server-red" />
  <img src="https://img.shields.io/badge/AI-OpenAI-00b894" />
  <img src="https://img.shields.io/badge/Messaging-Telegram-2CA5E0" />
</p>

---

<h2>🧩 Overview</h2>

<p>
  <strong>FreedomSupportBot</strong> is an AI-driven customer support system that connects a Telegram bot to an ASP.NET Core backend and the OpenAI API.
  It stores customers, conversations, and messages in SQL Server, uses conversation history to generate context-aware replies, and loads a configurable support persona and FAQ from an external file.
</p>

<ul>
  <li>Persistent customers, conversations, and message history</li>
  <li>Context-aware support replies using recent conversation history</li>
  <li>Configurable support persona and FAQ for different business use cases</li>
  <li>Clean service architecture ready to evolve into a multi-client platform</li>
</ul>

---

<h2>✨ Features</h2>

<h3>🔹 AI-Powered Support</h3>
<ul>
  <li>Integrates with the OpenAI Chat Completion API</li>
  <li>Uses a configurable persona and FAQ for domain-specific support</li>
  <li>Builds prompts using the latest conversation messages and the current user request</li>
</ul>

<h3>🔹 Conversation &amp; Message Management</h3>
<ul>
  <li>Each customer can have multiple conversations (sessions)</li>
  <li>Each conversation stores a full message history</li>
  <li>Messages record direction (customer or bot), text, and timestamp</li>
</ul>

<h3>🔹 Telegram Integration</h3>
<ul>
  <li>Uses the Telegram.Bot client library</li>
  <li><code>TelegramBotService</code> (a <code>BackgroundService</code>) receives and processes updates</li>
  <li>Scoped services (via dependency injection) handle each incoming message</li>
</ul>

<h3>🔹 Clean Service Architecture</h3>
<ul>
  <li><code>IAiSupportService</code> – handles OpenAI calls (HTTP + JSON)</li>
  <li><code>IConversationService</code> – customer, conversation, and message logic</li>
  <li><code>TelegramBotService</code> – background receiver and dispatcher</li>
  <li><code>FreedomSupportDbContext</code> – EF Core DbContext for persistence</li>
</ul>

---

<h2>🏗️ Architecture Overview</h2>

<pre>
Telegram User
    │
    ▼
Telegram Bot (Telegram API)
    │
    ▼
TelegramBotService (BackgroundService)
    │
    ▼  [creates scope]
IConversationService
    │
    ├─ GetOrCreateCustomerAsync
    ├─ GetOrCreateActiveConversationAsync
    ├─ SaveCustomerMessageAsync / SaveBotMessageAsync
    └─ HandleMessageAsync
            │
            ▼
      IAiSupportService (OpenAiSupportService)
            │
            ▼
        OpenAI API
</pre>

---

<h2>🗄️ Data Model (Simplified)</h2>

<pre>
Customer
  ├─ Id
  ├─ TelegramUserId
  ├─ Username
  ├─ FirstSeen
  └─ LastSeen
        │
        └── 1 : N
             Conversation
               ├─ Id
               ├─ CustomerId
               ├─ StartedAt
               ├─ EndedAt
               └─ IsActive
                     │
                     └── 1 : N
                          SupportMessage
                            ├─ Id
                            ├─ ConversationId
                            ├─ FromCustomer
                            ├─ Text
                            └─ CreatedAt
</pre>

---

<h2>⚙️ Request Flow</h2>

<ol>
  <li>User sends a message to the Telegram bot.</li>
  <li><code>TelegramBotService</code> receives the update and creates a scoped <code>IConversationService</code>.</li>
  <li><code>ConversationService</code>:
    <ul>
      <li>Gets or creates the <code>Customer</code></li>
      <li>Gets or creates the active <code>Conversation</code></li>
      <li>Saves the user <code>SupportMessage</code></li>
    </ul>
  </li>
  <li>Recent messages are loaded and turned into a conversation log string.</li>
  <li>A prompt is built combining:
    <ul>
      <li>persona + FAQ</li>
      <li>conversation history</li>
      <li>latest user message</li>
    </ul>
  </li>
  <li><code>IAiSupportService</code> sends the prompt to OpenAI and returns a reply.</li>
  <li>The reply is saved as a bot <code>SupportMessage</code> and sent back via Telegram.</li>
</ol>

---

<h2>🧠 Persona &amp; FAQ</h2>

<p>
  The bot’s behavior, tone, and domain knowledge are defined in an external persona file, for example:
</p>

<pre>Config/Persona/FreedomSupportPersona.txt</pre>

<p>This file typically contains:</p>
<ul>
  <li>Role (support agent for the Freedom job marketplace)</li>
  <li>Behavioral guidelines (clear answers, no invented rules)</li>
  <li>Key FAQs (how to create listings, why listings may not be visible, edit/delete rules, worker category rules)</li>
</ul>

<p>
  The persona is loaded at startup and used as the system message in OpenAI calls.
  This makes it easy to tweak behavior without changing code, and to swap personas per deployment or per client in the future.
</p>

---

<h2>🧰 Tech Stack</h2>

<table>
  <tr>
    <td><strong>Language</strong></td>
    <td>C#</td>
  </tr>
  <tr>
    <td><strong>Framework</strong></td>
    <td>ASP.NET Core MVC</td>
  </tr>
  <tr>
    <td><strong>ORM</strong></td>
    <td>Entity Framework Core</td>
  </tr>
  <tr>
    <td><strong>Database</strong></td>
    <td>SQL Server</td>
  </tr>
  <tr>
    <td><strong>Messaging</strong></td>
    <td>Telegram Bot API (Telegram.Bot)</td>
  </tr>
  <tr>
    <td><strong>AI</strong></td>
    <td>OpenAI Chat Completion API</td>
  </tr>
  <tr>
    <td><strong>Infrastructure</strong></td>
    <td>Hosted Services, Dependency Injection, Configuration</td>
  </tr>
</table>

---

<h2>🚧 Status &amp; Next Steps</h2>

<p><strong>Implemented:</strong></p>
<ul>
  <li>Telegram bot integration</li>
  <li>AI reply pipeline with conversation context</li>
  <li>Customer / Conversation / Message persistence</li>
  <li>Configurable persona + FAQ via external file</li>
</ul>

<p><strong>Planned:</strong></p>
<ul>
  <li>Command to end conversations cleanly</li>
  <li>Simple web admin panel to browse conversations</li>
  <li>Multi-client support (per-business personas and configuration)</li>
</ul>
