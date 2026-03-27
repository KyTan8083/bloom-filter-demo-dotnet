What this example does

Scenario: you have a set of known keys, like user:123, email:test@example.com, order:abc.

When a request comes in:

Check Bloom filter first
If Bloom says definitely not exists → return quickly
If Bloom says might exist → check authoritative store (DB / repository)
If actual store says not found, that is a false positive
Metrics are updated automatically
A background service prints real-time metrics every few seconds