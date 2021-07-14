param ($profile='blambda-dev', $domain='blambda')

aws --profile $profile `
		s3 rb --force s3://$domain `

aws --profile $profile `
		cloudformation delete-stack `
			--stack-name "$domain-stack" `
