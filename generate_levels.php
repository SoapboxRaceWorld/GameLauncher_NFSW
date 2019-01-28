<pre>
<?php

$lvl = (int)$_GET['lvl'] == 0 ? 60 : (int)$_GET['lvl'];

$array[1] = 100;
$array[2] = 875;
$array[3] = 1050;
$array[4] = 1600;
$array[5] = 2250;
$array[6] = 3000;
$array[7] = 3850;
$array[8] = 4800;
$array[9] = 5850;
$array[10] = 7000;

echo "INSERT INTO `LEVEL_REP` (`level`, `expPoint`) VALUES".PHP_EOL;

for ($i=1; $i <= $lvl; $i++) {
	if($i <= 10) {
		$formula = $array[$i];
	} elseif($i <= 30) {
		$start = 7000;
		$base = 1800;
		$inc = 200;

		$increase = $increase+($base+$inc)+(($inc*1)*($i-10));
		$formula = ($start-$base-$inc)+($increase)-($inc*($i-10))+($base+($inc*1));
	} elseif($i <= 50) {
		$start = 10600;
		$base = 1100;
		$inc = 400;

		$increase = $increase+($base+$inc)+(($inc*1)*($i-10));
		$formula = ($start-$base-$inc)+($increase)-($inc*($i-10))+($base+($inc*2));
	} else {
		$start = 27600;
		$base = 990;
		$inc = 440;

		$increase = $increase+($base+$inc)+(($inc*1)*($i-10));
		$formula = ($start-$base-$inc)+($increase)-($inc*($i-10))+14380;
	}

	$continuous .= "(".$i.", ".$formula."),".PHP_EOL;
}

echo substr($continuous, 0, -2).";";