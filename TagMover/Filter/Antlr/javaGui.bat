java org.antlr.v4.Tool -o JavaFolder FilterQuery.g4 -no-listener -visitor
cd JavaFolder
javac FilterQuery*.java
java org.antlr.v4.gui.TestRig FilterQuery query test.txt -gui