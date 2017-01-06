In this folder is a config.json.

By editing this with your favorite text editor, you can define what namespace keyword you want for your package. For example, this config.json defines a namespace "my", which means the haptics in this folder live in the "my" namespace. 

Why is this helpful? 

If you create a sequence called "beating_heart.sequence", then in code you can refer to it
as "my.beating_heart". If you write a pattern and want to refer to this sequence, you can also use "my.beating_heart". 

You can also refer to other packages to facilitate reuse of haptics. If you want to use any of the sequences in NS Core (namespace "ns") in your haptics, just write "ns.[haptics_name]". Remember that if you change your namespace after having written some haptics, you must update your haptics appropriately or else the references will be broken and you will receive errors. 

