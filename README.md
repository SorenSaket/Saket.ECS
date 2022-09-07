


Work in progress Archetype Entity Component System (ECS) in C#



TODO
<ul>
	<li>Query Caching</li>
	<li>Defer Spawn/Destroy Commands? Ensure in system command execution is possible to allow for lockless parallel execution.</li>
	<li>Parallel System Execution</li>
	<li>True SOA Storage to allow SIMD</li>
		To allow this queries should be able to match destoyed entities. This is to avoid sparse data in case of SIMD, but increases default use case complexity. To avoid unintended behavior implement Active Component.	
	<li>Extensive Testing</li>
	<li>Benchmark</li>
</ul>

Nice To have
	Built-in transform hierarchy