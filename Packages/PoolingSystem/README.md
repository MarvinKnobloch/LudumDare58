Create a empty gameobject and add the PoolingSystem script to it.

Add the IPoolingList interface to the object you want to pool.
Required interface values:  	public PoolingSystem.PoolObjectInfo poolingList { get; set; }

Use this line to spawn something with the pooling system:    PoolingSystem.SpawnObject(#PREFAB#, #SPAWNPOSITION#, #ROTATION#, #PARENT#(optional))
Use this line to return the object to the pooling system:    PoolingSystem.ReturnObjectToPool(gameObject, poolingList);
